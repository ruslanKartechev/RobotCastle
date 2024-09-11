using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackRangeSquare : IAttackRange
    {
        private List<Vector2Int> _cellsMask;

        public AttackRangeSquare(int halfWidth, int halfHeight)
        {
            _cellsMask = new List<Vector2Int>(halfWidth * halfHeight * 4);
        }

        public List<Vector2Int> GetCellsMask()
        {
            return _cellsMask;
        }

        public List<Vector2Int> GetRangeCells(Vector2Int center)
        {
            var result = new List<Vector2Int>();

            return result;
        }

        public List<Vector2Int> GetRangeCells(Vector2Int center, int maxX, int maxY)
        {
            var result = new List<Vector2Int>();

            return result;
        }
    }
}