using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IAttackRange
    {
        List<Vector2Int> GetCellsMask();
        List<Vector2Int> GetRangeCells(Vector2Int center);
        List<Vector2Int> GetRangeCells(Vector2Int center, int maxX, int maxY);
    }
}