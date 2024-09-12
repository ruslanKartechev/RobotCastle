using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackRangeSingle : IAttackRange
    {
        private List<Vector2Int> _cellsMask;

        public AttackRangeSingle()
        {
            _cellsMask = new List<Vector2Int>(4)
            {
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
            };
        }

        public List<Vector2Int> GetCellsMask() => _cellsMask;

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