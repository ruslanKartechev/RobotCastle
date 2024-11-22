using SleepDev;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class ScrollSave
    {
        public string id ;
        public int ownedAmount;
        public int purchasedCount;
        public DateTimeData timerData;
            
        public ScrollSave(){}

        public ScrollSave(string id, int amount)
        {
            this.id = id;
            this.ownedAmount = amount;
            purchasedCount = 0;
            timerData = new();
        }
        
        public ScrollSave(ScrollSave other)
        {
            id = other.id;
            ownedAmount = other.ownedAmount;
            purchasedCount = other.purchasedCount;
            timerData = new DateTimeData(other.timerData);
        }
    }
}