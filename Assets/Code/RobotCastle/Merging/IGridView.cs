namespace RobotCastle.Merging
{
    public interface IGridView
    {
        int GridId { get;}
        ICellView[,] Grid { get; }
        ICellView GetCell(int x, int y);
        void BuildGrid(MergeGrid gridData);
        MergeGrid BuildGridFromView();
        MergeGrid BuiltGrid { get; }
        
    }
}