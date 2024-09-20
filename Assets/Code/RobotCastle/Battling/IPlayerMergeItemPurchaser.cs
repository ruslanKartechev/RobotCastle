namespace RobotCastle.Battling
{
    public interface IPlayerMergeItemPurchaser
    {
        void TryPurchaseItem(bool promptUser = true);
    }
}