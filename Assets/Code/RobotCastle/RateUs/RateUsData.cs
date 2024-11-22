using System;
using System.Globalization;
using SleepDev;

namespace MergeHunt
{
    [System.Serializable]
    public class RateUsData
    {
        public const int LevelIndexToShowFirst = 3;
        public const int NextShowDelay = 3;
        
        public bool didShow;
        public bool didAccept;
        public DateTimeData nextShowTime;

        public RateUsData(){}

        public RateUsData(RateUsData other)
        {
            didAccept = other.didAccept;
            didShow = other.didShow;
            nextShowTime = new DateTimeData(other.nextShowTime);
        }

        public void SetAsRejected()
        {
            didShow = true;
            didAccept = false;
            SetNextShowTime();
        }

        public void SetAsAccepted()
        {
            didAccept = true;
            didShow = true;
        }
        
        public void SetNextShowTime()
        {
            var dd = DateTime.Now + new TimeSpan(days: NextShowDelay, hours: 0, minutes: 0, seconds: 0);
            CLog.Log($"Next rate us popup show time: {dd.ToString(CultureInfo.InvariantCulture)}");
            nextShowTime = new DateTimeData(dd);
        }
        
        public bool ShallShow(int levelIndex)
        {
            if (didAccept)
                return false;
            if (!didShow)
            {
                return levelIndex >= LevelIndexToShowFirst;
            }
            if (!didAccept)
            {
                if (nextShowTime.IsNull())
                {
                    CLog.Log("[RateUsData] nextShowTime.IsNone == true, possible error");
                    return true;
                }
                return DateTime.Now >= nextShowTime.GetDateTime();
            }
            return true;
        }
    }
}