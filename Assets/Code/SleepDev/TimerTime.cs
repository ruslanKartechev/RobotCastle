using System;

namespace SleepDev
{
    [System.Serializable]
    public struct TimerTime
    {
        public bool Equals(TimerTime other)
        {
            return Hour == other.Hour && Minute == other.Minute && Second == other.Second;
        }

        public override bool Equals(object obj)
        {
            return obj is TimerTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hour, Minute, Second);
        }

        public int Hour;
        public int Minute;
        public int Second;

        public static TimerTime Zero => new TimerTime(0, 0, 0);

        public TimerTime CorrectToZero()
        {
            if(Second < 0)
                Second = 0;
            if(Minute < 0)
                Minute = 0;
            if(Hour < 0)
                Hour = 0;
            return this;
        }
        
        public string TimeAsString
        {
            get
            {
                var result = $"{Hour:00}:{Minute:00}:{Second:00}";
                return result;
            }
        }

        public TimerTime(int hour, int minute, int second)
        {
            // Debug.Log($"*** *** Time Hour {hour}, Minute: {minute}, second: {second}");
            Hour = hour;
            Minute = minute;
            Second = second;
            CheckHour();
            CheckMinute();
            CheckSecond();
            // Debug.Log($"Corrected {Hour}, Minute: {Minute}, second: {Second}");
        }

        private void CheckHour()
        {
            if (Hour < 0)
                Hour = 0;   
        }

        private void CheckMinute()
        {
            if (Minute > 60)
            {
                Minute -= 60;
                Hour++;
                CheckHour();
            }
            
            if (Minute < 0)
            {
                if (Hour > 0)
                {
                    Hour--;
                    Minute += 60;   
                }
                else
                    Minute = 0;       
            }
        }

        private void CheckSecond()
        {
            if (Second > 60)
            {
                Minute++;
                Second -= 60;
                CheckMinute();
            }

            if (Second < 0)
            {
                if (Minute > 0)
                {
                    Minute--;
                    Second += 60;
                }
                else
                    Second = 0;
            }   
        }
        

        public TimerTime(System.DateTime dateTime)
        {
            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
        }

        public static TimerTime operator +(TimerTime a, TimerTime b)
        {
            return new TimerTime(a.Hour + b.Hour, a.Minute + b.Minute, a.Second + b.Second);
        }
        
        public static TimerTime operator -(TimerTime a, TimerTime b)
        {
            return new TimerTime(a.Hour - b.Hour, a.Minute - b.Minute, a.Second - b.Second);
        }
        
        
        public static bool operator ==(TimerTime a, TimerTime b)
        {
            return (a.Hour == b.Hour) && (a.Minute == b.Minute) && (a.Second == b.Second);
        }
        
        public static bool operator !=(TimerTime a, TimerTime b)
        {
            return !(a == b);
        }
        
     
    }
}