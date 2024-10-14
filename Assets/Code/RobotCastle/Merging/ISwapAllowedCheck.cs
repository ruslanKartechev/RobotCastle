namespace RobotCastle.Merging
{
    public interface ISwapAllowedCheck
    {
        bool IsSwapAllowed(ICellView cell1, ICellView cell2);
    }
}