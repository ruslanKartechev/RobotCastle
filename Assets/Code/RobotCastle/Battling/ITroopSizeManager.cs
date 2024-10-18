using SleepDev.Data;

namespace RobotCastle.Battling
{
    public interface ITroopSizeManager
    {
        ReactiveInt NextCost { get;}
        bool CanPurchase();
        int GetCost();
        void ExtendBy(int count);
        
        /// <summary>
        /// </summary>
        /// <returns>0 if success. 1 if not enough money</returns>
        int TryPurchase();
        
    }
}