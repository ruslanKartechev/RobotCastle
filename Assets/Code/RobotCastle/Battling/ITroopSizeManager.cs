namespace RobotCastle.Battling
{
    public interface ITroopSizeManager
    {
        bool CanPurchase();
        int GetCost();
        
        /// <summary>
        /// </summary>
        /// <returns>0 if success. 1 if not enough money</returns>
        int TryPurchase();
    }
}