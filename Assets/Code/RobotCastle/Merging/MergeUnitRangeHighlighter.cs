using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeUnitRangeHighlighter
    {
        private int MinY = 2;
        private int MaxX = 7;
        private int MaxY = 20;

        private readonly List<CellHighlight> _highlights = new (10);
        
        private IGridView _grid;
        private HeroStatsManager _stats;
        private CellHighlight _underCell;
        private Vector2Int _currentPos;
        
        
        public MergeUnitRangeHighlighter(GameObject unit, IGridView grid)
        {
            _stats = unit.gameObject.GetComponent<HeroStatsManager>();
            _grid = grid;
            switch (grid.GridId)
            {
                case MergeConstants.PlayerGridId:
                    MinY = 2;
                    MaxY = 20;
                    MaxX = 7;
                    break;
                case MergeConstants.EnemyGridId:
                    MinY = -4;
                    MaxY = 12;
                    MaxX = 7;
                    break;
            }
        }

        public void ShowUnderCell(Vector2Int centerCoord)
        {
            _currentPos = centerCoord;
            if (_underCell == null)
            {
                _underCell = (CellHighlight)ServiceLocator.Get<ISimplePoolsManager>().GetOne(ObjectPoolConstants.UnderCellFxId);
            }
            _underCell.gameObject.SetActive(true);
            _underCell.transform.position = _grid.GetCell(centerCoord.x, centerCoord.y).WorldPosition;
        }

        public void UpdateForUnderCell(Vector2Int centerCoord)
        {
            _currentPos = centerCoord;
            if (centerCoord.y < MinY)
            {
                foreach (var cc in _highlights)
                    cc.gameObject.SetActive(false);
                return;
            }

            var mask = _stats.Range.GetCellsMask();
            mask.Remove(Vector2Int.zero);
            var count = mask.Count;
            var pool = ServiceLocator.Get<ISimplePoolsManager>();
            while (_highlights.Count < count)
            {
                _highlights.Add((CellHighlight)pool.GetOne(ObjectPoolConstants.HeroRangeFxId));
            }
            for (var i = 0; i < _highlights.Count; i++)
                _highlights[i].gameObject.SetActive(false);

            var worldCenter = _grid.GetCell(centerCoord.x, centerCoord.y).WorldPosition;
            var rotation = _grid.GetCell(0, 0).WorldRotation;
            // CLog.LogRed($"Center Coord: {centerCoord}");
            for (var i = 0; i < mask.Count; i++)
            {
                var dir = mask[i];
                var cellCoord = centerCoord + dir;
                if (cellCoord.x < 0  || cellCoord.x >= MaxX || cellCoord.y < MinY || cellCoord.y >= MaxY)
                    continue;
                var worldPos = worldCenter + rotation * new Vector3(dir.x, 0, dir.y);
                // CLog.Log($"Cell {cellCoord}. World {worldPos}");
                _highlights[i].transform.position = worldPos;
                _highlights[i].gameObject.SetActive(true);
            }
   
        }

        public void Clear()
        {
            var pool = ServiceLocator.Get<ISimplePoolsManager>();
            foreach (var cellHighlight in _highlights)
                pool.ReturnOne(cellHighlight);
            if(_underCell != null)
                pool.ReturnOne(_underCell);
        }
        
        
    }
}