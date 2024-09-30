using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    public interface IMap
    {
        List<IAgent> ActiveAgents { get; }
        public bool IsOutOfBounce(Vector2Int coord);
        Vector2Int Size { get; }
        MapCell[,] Grid { get; }
        void GetCellAtPosition(Vector3 worldPosition, out Vector2Int mapCoord, out MapCell cell);
        Vector2Int GetCellPositionFromWorld(Vector3 worldPosition);
        Vector3 GetWorldFromCell(Vector2Int cell);

        bool GetIfWalkable(Vector3 worldPosition);
        float CellSize {get;}
        Vector3 WorldOrigin { get; }

        /// <summary>
        /// Sets both isPlayerWalkable and isAIWalkable at given (x,y) grid cell
        /// </summary>
        void SetWalkable(int x, int y, bool walkable);
    }
}