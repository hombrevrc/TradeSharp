//+------------------------------------------------------------------+
//|                                         RumusCaymanTrader.mq4    |
//| Особенность этого робота - не допускает 2-х и более подряд       |
//| покупок / продаж по одному инструменту. Открыв покупку, закрывает|
//| продажу и - наоборот                                             |
//+------------------------------------------------------------------+
#property copyright "Copyright © 2016, Andrey Sitaev"
#property link      "http://www.metaquotes.net"

#import "UdpQueueLib.dll"
int StartListenAddr(string addr, int port);
int StopListen();
string PickMessage();
int GetPickFlag();
int SendMessageUDP(string str, string addr, int port);

extern int       portOwn = 8011;//порт, который слушает робот
extern string    address = "127.0.0.1";//адрес, куда отправлять команды
extern int       portTarget = 8010;//порт, куда отправлять команды
extern int       slipMilliseconds = 300;//интервал опроса, ms
extern double    slippage = 20;//допустимое проскальзывание, пп

//+------------------------------------------------------------------+
//| распарсить строку в массив                                       |
//+------------------------------------------------------------------+
void split(string& arr[], string str, string sym) 
{
   ArrayResize(arr, 0);

   string item;
   int pos, size;

   int len = StringLen(str);
   for (int i = 0; i < len;) 
   {
      pos = StringFind(str, sym, i);
      if (pos == -1) pos = len;

      item = StringSubstr(str, i, pos - i);
      item = StringTrimLeft(item);
      item = StringTrimRight(item);

      size = ArraySize(arr);
      ArrayResize(arr, size + 1);
      arr[size] = item;

      i = pos + 1;
   }
}

int GetOpenedOrderSign(string smb)
{
   for (int i = 0; i < 999; i++)
   {
      if (OrderSelect(i, SELECT_BY_POS) == false) break;
      int cmd = OrderType();
      if (cmd != OP_BUY && cmd != OP_SELL) continue;
      if (OrderSymbol() == smb)
      {
         return cmd;
      }
   }
   return -1;
}

//+------------------------------------------------------------------+
//| войти в рынок (попытка)                                          |
//| cmd = OP_BUY | OP_SELL                                           |
//+------------------------------------------------------------------+
void OpenPos(int cmd, string symb, int volume, string comment)
{
   double lots = volume / 100.0;
   double price, sl = 0, tp = 0;
   
   // если уже есть открытая сделка...
   // если открыта сделка другого знака - закрыть ее
   // если открыта сделка того же знака - игнорировать сигнал
   int openedSign = GetOpenedOrderSign(symb);
   if (openedSign == cmd)
   {
      Print("Try to open ", cmd == OP_BUY ? "BUY" : "SELL", " by ", symb, " - already opened");
      return;
   }
   
   if (openedSign != cmd && openedSign > -1)
   {
      // закрыть ордер
      Print("On command ", cmd == OP_BUY ? "BUY" : "SELL", " by ", symb, " closing the existing position");
      CloseCurrentPos(100);
   }
      
   if (cmd == OP_BUY) 
      price = MarketInfo(symb, MODE_ASK);
   else
      price = MarketInfo(symb, MODE_BID);
   
   for (int counter = 4; counter >= 0; counter--)
   {
      int ticket = OrderSend(symb, cmd, lots, price, slippage, sl, tp, comment, 0);   
      if (ticket >= 0)
      {
         Print("Order succeeded: ", cmd, " ", symb, " ", lots, " at ", 
            price, " comment is ", comment);
         return;
      }
      else
      {
         int err = GetLastError();
         Print("Order failed: ", cmd, " ", symb, " ", lots, " at ", 
            price, " comment is ", comment, ". Error code is ", err);
         if (counter > 0) Sleep(500);
      }
   }
}

void CloseCurrentPos(int percent)
{
   string symbol = OrderSymbol();
   string comment = OrderComment();
   int cmd = OrderType();
   double price = 0;
   if (cmd == OP_SELL) 
      price = MarketInfo(OrderSymbol(), MODE_ASK);
   else
      price = MarketInfo(OrderSymbol(), MODE_BID);
   Print("Exit price is ", price); 
   
   double lotsLeft = MathRound(OrderLots() * (100 - percent)) / 100;  
   if (!OrderClose(OrderTicket(), OrderLots(), price, slippage))
   {
      int errCode = GetLastError();
      Print("Can not close order. The reason code is ", errCode);
      return;
   }   
   
   if (lotsLeft > 0)
       OpenPos(cmd, symbol, lotsLeft * 100, comment);
}

//+------------------------------------------------------------------+
//| закрыть позицию                                                  |
//+------------------------------------------------------------------+
void CloseByLoginTicket(string login, string ticket, int percent)
{
   if (login != "") Print("Close pos by login ", login);
   else 
   {
      if (ticket != "") Print("Close pos by ticket ", ticket);
      else 
      {
         Print("No login - ticket is provided");
         return;
      }
   }
   
   string comment;
   string arr[];
   
   for (int i = 0; i < 999; i++)
   {
      if (OrderSelect(i, SELECT_BY_POS) == false) break;
      comment = OrderComment();
      if (comment == "") continue;
      split(arr, comment, "#");
      int count = ArraySize(arr);
      if (count != 2) continue;
      if (login != "" && login != arr[0]) continue;
      if (ticket != "" && ticket != arr[1]) continue;
      
      Print("Closing pos[", i, "] by ", percent, "%");
      // закрыть ордер
      CloseCurrentPos(percent);
      i--;
   }   
}

//+------------------------------------------------------------------+
//| закрыть позиции, не попавшие в список                            |
//+------------------------------------------------------------------+
void CloseNotActual(int targetLogin, string &arPtrs[])
{
   // сформировать массив magic-номеров позиций, которые должны остаться
   int ticketsCount = ArraySize(arPtrs) - 2;
   int magics[1] = { 0 };
   int i = 0;
   ArrayResize(magics, ticketsCount);
      
   for (i = 0; i < ticketsCount; i++)
   {
      int magic = StrToInteger(arPtrs[i + 2]);
      magics[i] = magic;
   }
   
   // пройтись по позициям...
   string arr[];
   for (i = 0; i < 999; i++)
   {
      if (OrderSelect(i, SELECT_BY_POS) == false) break;
      
      split(arr, OrderComment(), "#");
      int count = ArraySize(arr);
      if (count < 2) continue;
      int login = StrToInteger(arr[0]);
      if (login != targetLogin) continue;
      int ticket = StrToInteger(arr[1]);
      
      bool hasTicket = false;
      for (int j = 0; j < ticketsCount; j++)
      {
         if (magics[j] == ticket)
         {
            hasTicket = true;
            break;
         }
      }
      
      if (hasTicket) continue;
      
      // закрыть позицию
      CloseCurrentPos(100);
      i--;
   }  
}

//+------------------------------------------------------------------+
//| исполнить команду                                                |
//| CLOST_2348011 -                                                  |
//| закрыть ордер с комментарием вида XXXX#2348011                   |
//|                                                                  |
//| CLOSL_148179 -                                                   |
//| закрыть все ордера с комментарием вида 148179#YYYY               |
//|                                                                  |
//| BUY_105_USDCHF_148179#2348011 -                                  |
//| купить 1.05 лота USDCHF, выставить комментарий 148179#2348011    |
//|                                                                  |
//| PARTIAL_25_2348011 -                                             |
//| закрыть 25% объема ордера с комментарием вида 148179#2348011     |
//|                                                                  |
//| ACTLIST_148179_2348011_240071 -                                  |
//| закрыть все ордера с комментарием вида 148179#YYYY, не попавшие  |
//| в переданный список ордеров                                      |
//+------------------------------------------------------------------+
void ExecuteCommand(string cmdStr)
{
   Print(cmdStr);
   string arr[];
   split(arr, cmdStr, "_");
   int count;
           
   if (arr[0] == "CLOST")
   {
      count = ArraySize(arr) - 1;
      if (count < 1) return;
      CloseByLoginTicket("", arr[1], 100);
      return;
   }
   
   if (arr[0] == "CLOSL")
   {
      count = ArraySize(arr) - 1;
      if (count < 1) return;
      CloseByLoginTicket(arr[1], "", 100);
      return;
   }   
   
   if (arr[0] == "PARTIAL")
   {
      count = ArraySize(arr) - 1;
      if (count < 2) return;
      int percent = StrToInteger(arr[1]);
      CloseByLoginTicket("", arr[2], percent);   
      return;
   }
   
   if (arr[0] == "ACTLIST")
   {
      count = ArraySize(arr) - 1;
      if (count < 2) return;
      CloseNotActual(StrToInteger(arr[1]), arr);
      return;
   }
   
   if (arr[0] == "REPORT")
   {
      SendOrdersToTarget();
      return;
   }

   int cmd = OP_BUY;
   if (arr[0] == "SELL") cmd = OP_SELL;
   // cmd - smb - vol - comment
   OpenPos(cmd, arr[2], StrToInteger(arr[1]), arr[3]);
}

//+------------------------------------------------------------------+
//| отправить в адресату текущие средства и комментарии ордеров      |
//+------------------------------------------------------------------+
void SendOrdersToTarget()
{
   string strBal = StringConcatenate("EQT=", DoubleToStr(AccountEquity(), 0));
      
   // собрать строку 
   // пройтись по позициям...
   for (int i = 0; i < 999; i++)
   {
      if (OrderSelect(i, SELECT_BY_POS) == false) break;
      
      int sideType = OrderType();
      if (sideType != OP_BUY && sideType != OP_SELL) continue;
           
      string comment = OrderComment();        
      // ищу комментарий вида 148179#2013467
      if (comment == "") continue;
      if (StringFind(comment, "#") <= 0) continue;
            
      strBal = StringConcatenate(strBal, ";", comment); 
   }
   
   // отправить строку на сервер
   SendMessageUDP(strBal, address, portTarget);
}

//+------------------------------------------------------------------+
//| expert initialization function                                   |
//+------------------------------------------------------------------+
int init()
{
   if (StartListenAddr(address, portOwn) == 0)
      Print("Error in StartListenAddr");
   if (GetPickFlag() == 0)
      Print("Started listening ", address, ":", portOwn);
   else
      Print("Error in StartListenAddr: ", GetPickFlag());
   EventSetMillisecondTimer(50);
   return(0);
}

//+------------------------------------------------------------------+
//| expert deinitialization function                                 |
//+------------------------------------------------------------------+
int deinit()
{
   EventKillTimer();
   Print("Stopping listening");
   StopListen();
   Print("Stopped listening");
   return(0);
}

void OnTimer()
{
   string str = PickMessage();         
   if (StringLen(str) > 0) 
   {
      if (str != "REPORT")
            Comment(str);
      ExecuteCommand(str);
   }
}
