namespace RobotCastle.Core
{
    [System.Serializable]
    public class PlayerScrollsData
    {
        public int scrollsTyp1;
        public int scrollsTyp2;
        public int scrollsTyp3;
        public bool didTakeNoAds;
        public DateTimeData lastTimeNoAdsAccepted;
        

        public PlayerScrollsData(PlayerScrollsData other)
        {
            scrollsTyp1 = other.scrollsTyp1;
            scrollsTyp2 = other.scrollsTyp2;
            scrollsTyp3 = other.scrollsTyp3;
            didTakeNoAds = other.didTakeNoAds;
            lastTimeNoAdsAccepted = new DateTimeData(other.lastTimeNoAdsAccepted);
        }

    }
}