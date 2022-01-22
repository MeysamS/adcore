using System;
using System.Globalization;

namespace ADCore.Kafka.Utilities
{
    public class Pdate
    {
        private static readonly int Shanbeh = (int)DayOfWeek.Saturday;
        private static readonly Calendar PerCal = new PersianCalendar();
        private static readonly CultureInfo PerInfo = new CultureInfo("fa-IR");

        public static DateTimeOffset GetStartOfUtcToday()
        {
            var d = DateTimeOffset.Now.Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetEndOfUtcToday()
        {
            var d = DateTime.Now.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1)).Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetStartOfUtcTomorrow()
        {
            var d = DateTime.Now.AddDays(1).Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetStartOfUtcWeek()
        {
            var dow = (int)PerCal.GetDayOfWeek(DateTime.Now);
            var diff = (7 + dow - Shanbeh) % 7;
            var date = DateTime.Now.Subtract(TimeSpan.FromDays(diff));
            var d = date.Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetStartOfUtcNextWeek()
        {
            var dow = (int)PerCal.GetDayOfWeek(DateTime.Now);
            var diff = (7 + dow - Shanbeh) % 7;
            var date = DateTime.Now.Subtract(TimeSpan.FromDays(diff));
            var d = date.Date.AddDays(7).ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetStartOfUtcMonth()
        {
            var dom = PerCal.GetDayOfMonth(DateTime.Now);
            var diff = dom - 1;
            var date = DateTime.Now.Subtract(TimeSpan.FromDays(diff));
            var d = date.Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetStartOfUtcNextMonth()
        {
            var dom = PerCal.GetDayOfMonth(DateTime.Now);
            var year = PerCal.GetYear(DateTime.Now);
            var month = PerCal.GetMonth(DateTime.Now);
            var daysInMonth = PerCal.GetDaysInMonth(year, month);
            var diff = daysInMonth - dom + 1;
            var date = DateTime.Now.Add(TimeSpan.FromDays(diff));
            var d = date.Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetStartOfUtcYear()
        {
            var dom = PerCal.GetDayOfYear(DateTime.Now);
            var diff = dom - 1;
            var date = DateTime.Now.Subtract(TimeSpan.FromDays(diff));
            var d = date.Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetStartOfUtcNextYear()
        {
            var dom = PerCal.GetDayOfYear(DateTime.Now);
            var year = PerCal.GetYear(DateTime.Now);
            var daysInYear = PerCal.GetDaysInYear(year);
            var diff = daysInYear - dom + 1;
            var date = DateTime.Now.Add(TimeSpan.FromDays(diff));
            var d = date.Date.ToUniversalTime();
            return d;
        }

        public static DateTimeOffset GetUtcFromLocalString(string value)
        {
            return DateTimeOffset.TryParse(value, PerInfo, DateTimeStyles.AssumeLocal, out var dto)
                ? dto.ToUniversalTime()
                : throw new ArgumentException($"The value {value} is not a valid persian date time object.");
        }

    }
}
