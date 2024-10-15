using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public interface IPlayerItemSpawnModifier
    {
        void OnNewItemSpawned(IItemView view);
        
    }
}