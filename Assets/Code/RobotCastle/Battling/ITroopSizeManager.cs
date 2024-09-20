namespace RobotCastle.Battling
{
    public interface ITroopSizeManager
    {
        bool CanPurchase();
        int GetCost();
        int TryPurchase();
    }
}