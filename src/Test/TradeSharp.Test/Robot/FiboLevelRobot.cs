using System.Collections.Generic;
using Entity;
using NUnit.Framework;
using TradeSharp.Contract.Util.BL;
using TradeSharp.Robot.BacktestServerProxy;
using TradeSharp.Robot.Robot;
using TradeSharp.Util;

namespace TradeSharp.Test.Robot
{
    //[TestFixture]
    public class NuFiboLevelRobot
    {
        private RobotContextBacktest context;

        #region Свечи m1
        #endregion

        //[SetUp]
        public void InitContext()
        {
            // подготовить тестовые котировки

            // подготовить тестовый контекст
            context = new RobotContextBacktest((tickers, end) => { }, () => new List<Cortege2<string, BarSettings>>(0));
        }

        //[Test]
        public void TradeTest()
        {
            var bot = new FiboLevelRobot();
            bot.Initialize(context, CurrentProtectedContext.Instance);
        }
    }
}
