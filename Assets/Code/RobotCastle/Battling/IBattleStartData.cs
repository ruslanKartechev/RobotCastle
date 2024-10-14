using RobotCastle.Data;

namespace RobotCastle.Battling
{
    public interface IBattleStartData
    {
        int StartMoney { get; }
        void AddMoney(int money);
        void AddStartItem(CoreItemData item);
    }
    
}