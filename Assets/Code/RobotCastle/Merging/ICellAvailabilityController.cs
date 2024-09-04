using UnityEngine;

namespace RobotCastle.Merging
{
    public interface ICellAvailabilityController
    {
        bool IsCellAllowed(int x, int y, ItemData item);
        void OnGridUpdated(MergeGrid grid);
        bool GetFreeCell(MergeGrid grid, out Vector2Int coordinates);
        int GetFreeCellsCount(MergeGrid grid);
        
    }
}