using UnityEngine;

namespace Bomber
{
    public interface IMap
    {
        Vector2Int Size { get; }
        MapCell[,] Grid { get; }
        void GetCellAtPosition(Vector3 worldPosition, out Vector2Int mapCoord, out MapCell cell);
        bool GetIfWalkable(Vector3 worldPosition);
        float CellSize {get;}
        Vector3 WorldOrigin { get; }

        /// <summary>
        /// Sets both isPlayerWalkable and isAIWalkable at given (x,y) grid cell
        /// </summary>
        void SetWalkable(int x, int y, bool walkable);
    }
}