using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Entity;
using Newtonsoft.Json;
using TradeSharp.Contract.Entity;
using TradeSharp.Contract.Util.Proxy;
using TradeSharp.RobotFarm.Request;
using TradeSharp.Util;
using Position = TradeSharp.RobotFarm.Request.Position;

namespace TradeSharp.RobotFarm.BL.Web
{
    partial class WebServer
    {
        private void ProcessJSONPostRequest(HttpListenerContext context)
        {
            var responseString = JsonConvert.SerializeObject(ProcessJSONPostRequestBody(context));
            var resp = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentType = "text/xml";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentLength64 = resp.Length;
            context.Response.OutputStream.Write(resp, 0, resp.Length);
        }

        private JsonResponse ProcessJSONPostRequestBody(HttpListenerContext context)
        {
            if (!context.Request.HasEntityBody)
            {
                Logger.Error("JSONPost: body was not provided");
                return new JsonResponse(false, "body was not provided");
            }

            string line;
            using (var sr = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                line = sr.ReadToEnd();
                if (string.IsNullOrEmpty(line))
                {
                    Logger.Error("JSONPost: body was empty");
                    return new JsonResponse(false, "body was empty");
                }
            }

            // распарсить объект запроса
            var req = DeserializeRequest(line);
            if (req == null)
            {
                Logger.Error("JSONPost: body was not parsed (" + line + ")");
                return new JsonResponse(false, "body was not recognized as a correct request");
            }

            if (req is RequestAccounts)
                return ProcessRequestAccounts((RequestAccounts) req);

            if (req is RequestLastOrders)
                return ProcessRequestLastOrders((RequestLastOrders)req);

            if (req is RequestPositionsClosing)
                return ProcessRequestPositionsClosing((RequestPositionsClosing)req);

            if (req is RequestPositionsModifying)
                return ProcessRequestPositionsModifying((RequestPositionsModifying)req);
            
            if (req is RequestAccountActualizing)
                return ProcessActualizeAccounts();

            Logger.Info("not implemented: " + (line.Length > 512 ? line.Substring(0, 512) : line));
            return new JsonResponse(false, "not implemented")
            {
                RequestId = req.RequestId
            };
        }

        private JsonResponse ProcessRequestAccounts(RequestAccounts req)
        {
            var response = new JsonResponseAccounts
            {
                Success = true,
                RequestId = req.RequestId,
                Accounts = RobotFarm.Instance.Accounts.Select(a =>
                    {
                        var ac = new JsonResponseAccounts.Account
                        {
                            Id = a.AccountId,
                            Login = a.UserLogin,
                            Password = a.UserPassword
                        };
                        var ctx = a.GetContext();
                        if (ctx != null)
                        {
                            var acInfo = ctx.AccountInfo;
                            if (acInfo != null)
                            {
                                ac.Balance = acInfo.Balance;
                                ac.Equity = acInfo.Equity;
                            }
                        }
                        ac.Robots = a.Robots.Select(r =>
                            new JsonResponseAccounts.AccountRobot
                            {
                                Name = r.TypeName,
                                TickerTimeframe = string.Join(", ", r.Graphics.Select(g => string.Format("{0} {1}",
                                    g.a, BarSettingsStorage.Instance.GetBarSettingsFriendlyName(g.b))))
                            }).ToList();
                        return ac;
                    }).ToList(),
                FarmState = RobotFarm.Instance.State
            };

            return response;
        }

        private JsonResponse ProcessRequestLastOrders(RequestLastOrders req)
        {
            var response = new JsonResponseLastOrders
            {
                Success = true,
                RequestId = req.RequestId,
                AccountPositions = RobotFarm.Instance.Accounts.ToDictionary(a => a.AccountId,
                    a => a.GetAccountOrders().Select(o => new Position
                    {
                        Id = o.ID,
                        AccountId = o.AccountID,
                        Side = o.Side,
                        PriceEnter = (decimal)o.PriceEnter,
                        PriceExit = (decimal)(o.PriceExit ?? 0),
                        Profit = (decimal)o.ResultDepo,
                        TimeEnter = o.TimeEnter,
                        TimeExit = o.TimeExit ?? default(DateTime),
                        Sl = (decimal)(o.StopLoss ?? 0),
                        Tp = (decimal)(o.TakeProfit ?? 0),
                        Volume = o.Volume,
                        VolumeDepo = (decimal)o.VolumeInDepoCurrency,
                        Symbol = o.Symbol,
                        Mt4Order = o.MasterOrder ?? 0
                    }).ToList())
            };

            return response;
        }

        private JsonResponse ProcessRequestPositionsClosing(RequestPositionsClosing req)
        {
            var resp = new JsonResponsePositionsClosing
            {
                RequestId = req.RequestId
            };
            int countOk, countFailed;
            string errors;
            ModifyOrders(req.positions, true, out countOk, out countFailed, out errors);
            resp.CountClosed = countOk;
            resp.CountFail = countFailed;
            resp.ErrorString = errors;
            resp.Success = countOk > countFailed;
            return resp;            
        }

        private JsonResponse ProcessRequestPositionsModifying(RequestPositionsModifying req)
        {
            var resp = new JsonResponsePositionsModifying
            {
                RequestId = req.RequestId
            };
            int countOk, countFailed;
            string errors;
            ModifyOrders(req.positions, false, out countOk, out countFailed, out errors);
            resp.CountModified = countOk;
            resp.CountFail = countFailed;
            resp.ErrorString = errors;
            resp.Success = countOk > countFailed;
            return resp;
        }

        private JsonResponse ProcessActualizeAccounts()
        {
            //RobotFarm.Instance.Accounts.ForEach(a => a.);
            return new JsonResponse(true, string.Empty);
        }

        private JsonRequest DeserializeRequest(string requestJson)
        {
            return JsonRequest.ParseCommand(requestJson);
        }

        private void ModifyOrders(List<Position> positions, bool closeOrders,
            out int countModified, out int countFailed, out string errors)
        {
            countModified = 0;
            countFailed = 0;
            var errorStrings = new Dictionary<string, string>();
            try
            {
                foreach (var pos in positions)
                {
                    string errorString;
                    var order = new MarketOrder
                    {
                        AccountID = pos.AccountId,
                        ID = pos.Id,
                        Side = pos.Side,
                        Symbol = pos.Symbol,
                        StopLoss = (float)pos.Sl,
                        TakeProfit = (float)pos.Tp,
                        TimeEnter = pos.TimeEnter,
                        Volume = pos.Volume,
                        PriceEnter = (float)pos.PriceEnter,
                        MasterOrder = pos.Mt4Order
                    };
                    order.State = closeOrders ? PositionState.Closed : PositionState.Opened;
                    if (closeOrders)
                    {
                        order.TimeExit = pos.TimeExit;
                        order.PriceExit = pos.PriceExit == 0 ? (float?) null : (float) pos.PriceExit;
                        order.ResultDepo = (float) pos.Profit;
                    }
                    if (PlatformManager.Instance.proxy.ModifyOrder(order, out errorString))
                        countModified++;
                    else countFailed++;
                    if (!string.IsNullOrEmpty(errorString) && !errorStrings.ContainsKey(errorString))
                        errorStrings.Add(errorString, string.Empty);
                }
                errors = string.Join(". ", errorStrings.Keys);
                Logger.InfoFormat("ModifyOrders({0} orders): {1} are updated, errors: {2}",
                    positions.Count, countModified, errors);
            }
            catch (Exception ex)
            {
                countModified = 0;
                countFailed = positions.Count;
                errors = ex.GetType().Name + ": " + ex.Message;
                Logger.ErrorFormat("Error in ModifyOrders({0} orders): {1}",
                    positions.Count, ex);
            }
        }
    }
}
