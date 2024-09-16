using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeUnitRangeHighlighter
    {
        private const int MinY = 2;
        private const int MaxX = 7;
        private const int MaxY = 20;

        private readonly List<CellHighlight> _highlights = new (10);
        
        private IGridView _grid;
        private HeroStatsContainer _stats;
        private CellHighlight _underCell;
        private Vector2Int _currentPos;
        
        
        public MergeUnitRangeHighlighter(GameObject unit, IGridView grid)
        {
            _stats = unit.gameObject.GetComponent<HeroStatsContainer>();
            _grid = grid;
        }

        public void ShowUnderCell(Vector2Int centerCoord)
        {
            _currentPos = centerCoord;
            if (_underCell == null)
            {
                _underCell = (CellHighlight)ServiceLocator.Get<ISimplePoolsManager>().GetOne(MergeConstants.UnderCellFxId);
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
                _highlights.Add((CellHighlight)pool.GetOne(MergeConstants.HeroRangeFxId));
            }
            for (var i = 0; i < _highlights.Count; i++)
                _highlights[i].gameObject.SetActive(false);

            var worldCenter = _grid.GetCell(centerCoord.x, centerCoord.y).WorldPosition;
            for (var i = 0; i < mask.Count; i++)
            {
                var dir = mask[i];
                var cell = centerCoord + dir;
                if (cell.x is < 0 or >= MaxX || cell.y is < MinY or >= MaxY)
                    continue;
                var pos = worldCenter + new Vector3(dir.x, 0, dir.y);
                _highlights[i].transform.position = pos;
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