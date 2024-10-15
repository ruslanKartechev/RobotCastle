using RobotCastle.Data;

namespace RobotCastle.Battling.SmeltingOffer
{
    public interface ISmeltModifier
    {
        CoreItemData ModifySmeltItemBeforeApplied(CoreItemData itemData);
    }
}