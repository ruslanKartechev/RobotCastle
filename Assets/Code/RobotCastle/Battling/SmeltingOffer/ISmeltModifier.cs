using RobotCastle.Merging;

namespace RobotCastle.Battling.SmeltingOffer
{
    public interface ISmeltModifier
    {
        void OnSmeltedWeapon(IItemView view);
    }
}