using System;
using TradeSharp.Util;

namespace QuoteManager.BL
{
    public static class StandardDaysOff
    {
        public static int startDayOff = AppConfig.GetIntParam("DaysOff.StartDay", (int)DayOfWeek.Saturday);

        public static int startHourOff = AppConfig.GetIntParam("DaysOff.StartHour", 0);

        public static int durationHours = AppConfig.GetIntParam("DaysOff.DurationHours", 48);

        public static bool IsDayOff(DateTime time)
        {
            return IsDayOff(time, startDayOff, startHourOff, durationHours);
        }

        public static bool IsDayOff(DateTime time, int startDayOffValue, int startHourOffValue, int durationHoursValue)
        {
            DateTime startWeekDay;
            try
            {
                startWeekDay = time.DayOfWeek == DayOfWeek.Sunday
                    ? time.AddDays(-6).Date
                    : time.AddDays(-(int)time.DayOfWeek).Date;
            }
            catch (ArgumentOutOfRangeException)
            {
                startWeekDay = time;
            }
            var startOff = startWeekDay.AddDays(startDayOff).AddHours(startHourOff);
            var endOff = startWeekDay.AddDays(startDayOff).AddHours(startHourOff + durationHours);

            return time >= startOff && time <= endOff;
        }
    }
}
