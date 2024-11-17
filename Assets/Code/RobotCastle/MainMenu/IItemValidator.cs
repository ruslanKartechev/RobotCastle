using RobotCastle.Merging;

namespace RobotCastle.MainMenu
{
    public interface IItemValidator
    {
        bool CheckIfValid(IItemView itemView);
    }
}