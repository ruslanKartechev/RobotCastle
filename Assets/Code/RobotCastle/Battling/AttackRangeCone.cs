using System.Collections.Generic;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackRangeCone : IAttackRange
    {
        public AttackRangeCone(int distance)
        {
            _cellsMask = new(10);
            var x = 0;
            var xMax = 1;
            for (var y = 1; y <= distance; y++)
            {
                for (x = -xMax / 2; x <= xMax / 2; x++)
                {
                    var coord = new Vector2Int(x , y);
                    _cellsMask.Add(coord);
                }
                xMax += 2;
            }
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
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < 0 || coord.y < 0)
                    continue;
                result.Add(coord);
            }
            return result;
        }

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
            // CLog.Log($"Max x {maxX}. MaxY {maxY}");
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
            // CLog.Log($"Max x {maxX}. MaxY {maxY}");
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
            var cells = ApplyMask(fromPoint);
            Vector2Int closest = default;
            var minD2 = int.MaxValue;
            foreach (var c in cells)
            {
                var d = (c - toPoint).sqrMagnitude;
                if (d < minD2)
                {
                    minD2 = d;
                    closest = c;
                }
            }
            return closest;
        }

        public string GetStringDescription() => "";

        private readonly List<Vector2Int> _cellsMask;

    }
}