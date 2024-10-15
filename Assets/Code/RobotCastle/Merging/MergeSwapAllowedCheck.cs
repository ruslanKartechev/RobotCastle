namespace RobotCastle.Merging
{
    public class MergeSwapAllowedCheck : ISwapAllowedCheck
    {
        public bool IsSwapAllowed(ICellView cell1, ICellView cell2) => true;
    }
}