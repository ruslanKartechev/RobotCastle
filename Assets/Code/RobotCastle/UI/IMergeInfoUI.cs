namespace RobotCastle.UI
{
    public interface IMergeInfoUI : IScreenUI
    {
        void ShowNotEnoughTroopSize(int count, int max);
        void ShowNotEnoughMoney();
        void ShowNotEnoughSpace();
    }
}