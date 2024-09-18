using System.Collections.Generic;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackRangeSingle : IAttackRange
    {
        private List<Vector2Int> _cellsMask;

        public AttackRangeSingle()
        {
            _cellsMask = new List<Vector2Int>(4) {
                new(-1, 0),
                new(1, 0),
                new(0, 1),
                new(0, -1),
                new(0, 0),
            };
        }

        public List<Vector2Int> GetCellsMask() => _cellsMask;

        public List<Vector2Int> ApplyMask(Vector2Int center)
        {
            var result = new List<Vector2Int>(_cellsMask.Count);
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x < 0 || coord.y < 0)
                    continue;
                result.Add(coord);
            }
            return result;
        }

        public List<Vector2Int> ApplyMask(Vector2Int center, int maxX, int maxY)
        {
            var result = new List<Vector2Int>(_cellsMask.Count);
            CLog.Log($"Max x {maxX}. MaxY {maxY}");
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < 0 || coord.y < 0)
                    continue;
                result.Add(coord);
            }
            return result;
        }
        
        /// <summary>
        /// Max Inclusive. Min Exclusive
        /// </summary>
        public List<Vector2Int> ApplyMask(Vector2Int center, int minX, int minY, int maxX, int maxY)
        {
            var result = new List<Vector2Int>(_cellsMask.Count);
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < minX || coord.y < minY)
                    continue;
                result.Add(coord);
            }
            return result;
        }
        
        public List<ICellView> ApplyMask(Vector2Int center, IGridView gridView)
        {
            var result = new List<ICellView>(_cellsMask.Count);
            var maxX = gridView.Grid.GetLength(0);
            var maxY = gridView.Grid.GetLength(1);
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < 0 || coord.y < 0)
                    continue;
                var cell = gridView.GetCell(coord.x, coord.y);
                result.Add(cell);
            }
            return result;
        }

        public List<ICellView> ApplyMask(Vector2Int center, IGridView gridView, int minX, int minY)
        {
            var result = new List<ICellView>(_cellsMask.Count);
            var maxX = gridView.Grid.GetLength(0);
            var maxY = gridView.Grid.GetLength(1);
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < minX || coord.y < minY)
                    continue;
                var cell = gridView.GetCell(coord.x, coord.y);
                result.Add(cell);
            }
            return result;
        }

        public Vector2Int GetClosestCell(Vector2Int fromPoint, Vector2Int toPoint)
        {
            var shortest = int.MaxValue;
            var output = fromPoint;
            for (var i = 0; i < 4; i++)
            {
                var point = toPoint + _cellsMask[i];
                var dist = (point - fromPoint).sqrMagnitude;
                if (dist < shortest)
                {
                    shortest = dist;
                    output = point;
                }
            }
            return output;
        }
    }
}