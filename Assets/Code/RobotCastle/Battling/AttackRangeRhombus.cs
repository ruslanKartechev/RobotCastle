using System.Collections.Generic;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackRangeRhombus : IAttackRange
    {
        private readonly int _sideLength;
        private readonly List<Vector2Int> _cellsMask;

        public AttackRangeRhombus(int sideLength)
        {
            _sideLength = sideLength;
            _cellsMask = new List<Vector2Int>(_sideLength * _sideLength);
            
            for (var x = -sideLength; x <= sideLength; x++)
            {
                for (var y = -sideLength; y <= sideLength; y++)
                {
                    if(Mathf.Abs(x) + Mathf.Abs(y) <= sideLength )
                        _cellsMask.Add(new Vector2Int(x,y));
                }
            }
        }

        public string GetStringDescription() => (_sideLength * 2).ToString();

        public List<Vector2Int> GetCellsMask()
        {
            return _cellsMask;
        }

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
            throw new System.NotImplementedException();
            var allCellsAround = new List<Vector2Int>(20);
            var closest = fromPoint;
            var minD2 = int.MaxValue;
            var d2 = int.MaxValue;
            foreach (var dir in _cellsMask)
            {
                var c = toPoint + dir;
                
            }
   
        }
        
    }
}