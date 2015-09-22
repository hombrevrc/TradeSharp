using System;

namespace TradeSharp.FakeUser.BL
{
    public static class TimeUtil
    {
        public static DateTime RoundTime(DateTime time, int timeframe)
        {
            var date = time.Date;
            var minutes = (int)(time - date).TotalMinutes / timeframe;
            return date.AddMinutes(minutes * timeframe);
        }
    }
}
