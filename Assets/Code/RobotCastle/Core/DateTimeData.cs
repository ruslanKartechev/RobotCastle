using System;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class DateTimeData
    {
        public DateTimeData(){}
        
        public DateTimeData(DateTime dt)
        {
            this.year = dt.Year;
            this.month = dt.Month;
            this.day = dt.Day;
            this.hour = dt.Hour;
            this.minute = dt.Minute;
            this.second = dt.Second;
            this.milliSecond = dt.Millisecond;
        }

        public DateTimeData(DateTimeData other)
        {
            this.year = other.year;
            this.month = other.month;
            this.day = other.day;
            this.hour = other.hour;
            this.minute = other.minute;
            this.second = other.second;
            this.milliSecond = other.milliSecond;
        }

        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;
        public int milliSecond;

        public static DateTimeData FromNow()
        {
            return new DateTimeData(DateTime.Now);
        }

        public bool IsNull() => year == 0 && month == 0 && day == 0 && hour == 0;

      
        public DateTime GetDateTime()
        {
            var dt = new DateTime(year, month, day, hour, minute, second, milliSecond);
            return dt;
        }

        public TimeSpan GetTimeSpan()
        {
            return new TimeSpan(day, hour, minute, second, milliSecond);
        }
        
        public bool CheckIfTimePassed(TimeSpan timeSpan)
        {
            if (IsNull()) return true;
            var diff = DateTime.Now - GetDateTime();
            return diff >= timeSpan;
        }
    }
}