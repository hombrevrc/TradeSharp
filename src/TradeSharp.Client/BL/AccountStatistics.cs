using System;
using System.Collections.Generic;
using System.Linq;
using TradeSharp.Client.Util;
using TradeSharp.Contract.Entity;
using TradeSharp.Util;

namespace TradeSharp.Client.BL
{
    class AccountStatistics : AccountEfficiency
    {
        /// <summary>
        /// суммарный результат по закрытым сделкам
        /// </summary>
        public float sumClosedResult;

        /// <summary>
        /// суммарный результат по открытым сделкам
        /// </summary>
        public float sumOpenResult;

        /// <summary>
        /// сумма всех пополнений/списаний за период
        /// </summary>
        public float sumDeltaBalance;

        /// <summary>
        /// среднегеом. доход за день
        /// </summary>
        public float ProfitGeomDay { get; set; }

        /// <summary>
        /// Расчёт вообще всей статистики
        /// </summary>
        public void Calculate(
            List<BalanceChange> balanceChanges, 
            List<MarketOrder> marketOrders, DateTime startDate)
        {
            // первый трансфер считаем за начальный баланс
            InitialBalance = (float)balanceChanges[0].SignedAmountDepo;
            balanceChanges.RemoveAt(0);
            if (listEquity.Count < 2)
            {
                return;
            }
            
            // считаем результат по открытым позициям
            var openPosList = PositionSummary.GetPositionSummary(marketOrders.Where(o => o.State == PositionState.Opened).ToList(),
                AccountStatus.Instance.AccountData.Currency, (float)AccountStatus.Instance.AccountData.Balance);
            sumOpenResult = openPosList[openPosList.Count - 1].Profit;
            // посчитать суммарный результат по сделкам (дельта эквити за вычетом пополнений/списаний)
            // все закрытые позиции
            var dueBalance = balanceChanges.Where(bc => bc.ValueDate >= listEquity[0].time &&
                (bc.ChangeType == BalanceChangeType.Deposit ||
                 bc.ChangeType == BalanceChangeType.Withdrawal)).ToList();

            var closedList = balanceChanges.Where(bc => bc.ValueDate >= listEquity[0].time &&
                (bc.ChangeType == BalanceChangeType.Profit || 
                bc.ChangeType == BalanceChangeType.Loss)).ToList();

            // результат по всем закрытым сделкам
            sumClosedResult = (float)closedList.Sum(bc => bc.SignedAmountDepo);

            // сумма всех неторговых операций
            sumDeltaBalance = (float)dueBalance.Sum(bc => bc.SignedAmountDepo);
            //CurrentBalance = InitialBalance + sumClosedResult + sumDeltaBalance + sumOpenResult;
            
            // получить список ROR
            var listROR = new List<Cortege2<DateTime, float>>();

            for (var i = 1; i < listEquity.Count; i++)
            {
                var endEquity = listEquity[i];
                var startEquity = listEquity[i - 1];
                if (startEquity.equity == 0) break;
                var deltaBalance = 0f;
                for (var j = 0; j < dueBalance.Count; j++)
                {
                    if (dueBalance[j].ValueDate > listEquity[i].time) break;
                    deltaBalance += (float)dueBalance[j].SignedAmountDepo;
                    dueBalance.RemoveAt(j);
                    j--;
                }
                var rateOfReturn = ((endEquity.equity - startEquity.equity - deltaBalance) / startEquity.equity);
                listROR.Add(new Cortege2<DateTime, float>(listEquity[i].time, rateOfReturn));
            }
            // убрать все 0-е значения от начала отсчета
            for (var i = 0; i < listROR.Count; i++)
                if (listROR[i].b == 0)
                {
                    listROR.RemoveAt(i);
                    i--;
                }
                else break;

            if (listROR.Count == 0) return;

            // получить кривые доходности и просудания на виртуальную 1000
            listProfit1000 = new List<EquityOnTime>();
            listDrawDown1000 = new List<EquityOnTime>();

            var startBalance1000 = 1000f;

            listProfit1000.Add(new EquityOnTime(startBalance1000, startDate));
            listDrawDown1000.Add(new EquityOnTime(0, startDate));

            foreach (var ret in listROR)
            {
                var deltaBalance1000 = startBalance1000 * ret.b;
                startBalance1000 += deltaBalance1000;

                listProfit1000.Add(new EquityOnTime(startBalance1000, ret.a));
                listDrawDown1000.Add(new EquityOnTime(deltaBalance1000 < 0 ? deltaBalance1000 : 0, ret.a));
            }


            // посчитать макс. проседание
            CalculateMaxDrawdown();
            // посчитать среднегеометрическую дневную, месячную и годовую доходность
            var avgROR = listROR.Average(ret => ret.b);
            ProfitGeomMonth = (float)Math.Pow(1 + avgROR, 20f) - 1;
            ProfitGeomYear = (float)Math.Pow(1 + avgROR, 250f) - 1;
            ProfitGeomDay = avgROR;
        }

        /// <summary>
        /// Расчёт только максимальной просадки
        /// </summary>
        public float CalculateMaxDrawdown(List<EquityOnTime> listProfit1000)
        {
            this.listProfit1000 = listProfit1000;

            if (Statistics == null)
                Statistics = new PerformerStat();
            CalculateMaxDrawdown();
            return Statistics.MaxRelDrawDown;
        }

        /// <summary>
        /// Первая производняа от Средств (т.е. прибыль / просадка)
        /// </summary>
        /// <returns>Дата, изменение баланса, баланс</returns>
        public List<Cortege3<DateTime, float, float>> GetEquityDifferential()
        {
            var result = new List<Cortege3<DateTime, float, float>>();
            List<EquityOnTime> source = listEquity;

            if (source.Count == 0)
                return result;

            result.Add(new Cortege3<DateTime, float, float>(source[0].time, 0, 0));

            for (var i = 0; i < source.Count - 1; i++)
            {
                var curBalance = source[i].equity;
                var curDiff = source[i + 1].equity - curBalance;

                result.Add(new Cortege3<DateTime, float, float>(source[i + 1].time, curDiff, curBalance));
            }

            return result;
        }

        private void CalculateMaxDrawdown()
        {
            Statistics.MaxRelDrawDown = 0;
            if (listProfit1000.Count == 0) return;
            
            for (var i = 0; i < listProfit1000.Count - 1;)
            {
                var tempDrawDown = 0f;
                var startBalance = listProfit1000[i].equity;
                if (startBalance == 0) continue;
                
                var j = i + 1;
                for (; j < listProfit1000.Count; j++)
                {
                    var curBal = listProfit1000[j].equity;
                    if (curBal >= startBalance) break;
                    var curDd = startBalance - curBal;
                    if (curDd > tempDrawDown) tempDrawDown = curDd;
                }
                i = j;

                tempDrawDown /= startBalance;
                if (tempDrawDown > Statistics.MaxRelDrawDown)
                    Statistics.MaxRelDrawDown = tempDrawDown;
            }
            Statistics.MaxRelDrawDown *= 100;
        }
    }    
}
