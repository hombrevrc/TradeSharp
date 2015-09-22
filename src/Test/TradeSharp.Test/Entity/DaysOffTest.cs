using System;
using System.Collections.Generic;
using NUnit.Framework;
using TradeSharp.Contract.Util.BL;

namespace TradeSharp.Test.Entity
{
    [TestFixture]
    public class DaysOffTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            DaysOff.Initialize(new List<DayOff>
            {
                new DayOff("WD", null, 0, 6, 0, 50)
            });
        }

        [Test]
        public void TestPositive()
        {
            var date = new DateTime(2015, 06, 13, 2, 4, 1);
            var isOff = DaysOff.Instance.IsDayOff(date);
            Assert.IsTrue(isOff);

            date = new DateTime(2015, 06, 14, 2, 4, 1);
            isOff = DaysOff.Instance.IsDayOff(date);
            Assert.IsTrue(isOff);
            
            date = new DateTime(2015, 06, 06, 00, 00, 01);
            isOff = DaysOff.Instance.IsDayOff(date);
            Assert.IsTrue(isOff);

            date = new DateTime(2015, 06, 07, 00, 00, 00);
            isOff = DaysOff.Instance.IsDayOff(date);
            Assert.IsTrue(isOff);
        }

        [Test]
        public void TestNegative()
        {
            var date = new DateTime(2015, 06, 12, 22, 4, 1);
            var isOff = DaysOff.Instance.IsDayOff(date);
            Assert.IsFalse(isOff);

            date = new DateTime(2015, 06, 15, 4, 4, 1);
            isOff = DaysOff.Instance.IsDayOff(date);
            Assert.IsFalse(isOff);

            date = new DateTime(2015, 06, 5, 23, 00, 01);
            isOff = DaysOff.Instance.IsDayOff(date);
            Assert.IsFalse(isOff);
        }
    }
}
