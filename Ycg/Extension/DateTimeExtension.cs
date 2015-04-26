using System;

namespace Ycg.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Gets the current month days.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>Days.</returns>
        public static int GetCurrentMonthDays(this DateTime dateTime)
        {
            int year = dateTime.Year;
            int month = dateTime.Month;
            switch (month)
            {
                case 2:
                    return DateTime.IsLeapYear(year) ? 29 : 28;
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                default:
                    return 0;
            } 
        }

        /// <summary>
        /// Get the weeks for date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The date time array.</returns>
        private static DateTime[] GetWeeksForDate(this DateTime dateTime)
        {
            int intWeek = (int)dateTime.DayOfWeek;
            DateTime[] dateArray = new DateTime[7];
            if (intWeek == (int)DayOfWeek.Sunday)
            {
                for (int dayNumber = 6; dayNumber >= 0; dayNumber--)
                {
                    dateArray[6 - dayNumber] = dateTime.AddDays(intWeek - dayNumber);
                }
            }
            else
            {
                for (int dayNumber = 1; dayNumber <= 7; dayNumber++)
                {
                    dateArray[dayNumber - 1] = dateTime.AddDays(dayNumber - intWeek);
                }
            }
            return dateArray;
        }

        /// <summary>
        /// Get Two Date Interval Time.
        /// </summary>
        /// <param name="startTime">Start Date.</param>
        /// <param name="endTime">End Date.</param>
        /// <returns>Default return two date day interval time.</returns>
        public static double GetTimeInterval(this DateTime startTime, DateTime endTime)
        {
            return startTime.GetTimeInterval(endTime, TimeType.Day);
        }

        /// <summary>
        /// Get Two Date Interval Time.
        /// </summary>
        /// <param name="startTime">Start Date.</param>
        /// <param name="endTime">End Date.</param>
        /// <param name="timeType">Interval time type.</param>
        /// <returns>According to the time type get the interval time.</returns>
        public static double GetTimeInterval(this DateTime startTime, DateTime endTime, TimeType timeType)
        {
            TimeSpan startTimeSpan = new TimeSpan(startTime.Ticks);
            TimeSpan endTimeSpan = new TimeSpan(endTime.Ticks);
            TimeSpan intervalTimeSpan = startTimeSpan.Subtract(endTimeSpan).Duration();
            return GetTimeIntervalValue(intervalTimeSpan, timeType);
        }

        /// <summary>
        /// Get the interval time.
        /// </summary>
        /// <param name="intervalTimeSpan">Interval time span.</param>
        /// <param name="timeType">Interval time type.</param>
        /// <returns>Interval time value.</returns>
        private static double GetTimeIntervalValue(TimeSpan intervalTimeSpan, TimeType timeType)
        {
            switch (timeType)
            {
                case TimeType.Year:
                    return default(double);
                case TimeType.Day:
                    return intervalTimeSpan.TotalDays;
                case TimeType.Hour:
                    return intervalTimeSpan.TotalHours;
                case TimeType.Minute:
                    return intervalTimeSpan.TotalMinutes;
                case TimeType.Second:
                    return intervalTimeSpan.TotalSeconds;
                default:
                    return intervalTimeSpan.TotalDays;
            }
        }
    }

    public enum TimeType
    {
        Year = 1,
        Day = 2,
        Hour = 3,
        Minute = 4,
        Second = 5
    }
}
