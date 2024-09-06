namespace RobotCastle.Merging
{
    public interface IMergeCellHighlightPool
    {
        CellHighlight GetOneType1();
        CellHighlight GetOneType2();
        
        void ReturnType1(CellHighlight obj);
        void ReturnType2(CellHighlight obj);
        
        int CurrentCount { get; }
        void Init();
    }
}