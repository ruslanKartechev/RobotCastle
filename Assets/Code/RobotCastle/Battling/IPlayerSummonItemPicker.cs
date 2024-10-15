using RobotCastle.Data;

namespace RobotCastle.Battling
{
    public interface IPlayerSummonItemPicker
    {
        CoreItemData GetNext();
    }
}