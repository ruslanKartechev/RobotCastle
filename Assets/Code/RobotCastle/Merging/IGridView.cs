﻿namespace RobotCastle.Merging
{
    public interface IGridView
    {
        ICellView[,] Grid { get; }
        ICellView GetCell(int x, int y);
        void BuildGrid(MergeGrid gridData);
        MergeGrid BuildGridFromView();
        
    }
}