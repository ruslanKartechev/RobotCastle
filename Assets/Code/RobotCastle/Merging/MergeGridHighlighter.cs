using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeGridHighlighter : MonoBehaviour
    {
        private IGridView _gridView;
        private IMergeProcessor _mergeProcessor;
        private List<ICellView> _highlightedCells;

        public void Init(IGridView gridView, IMergeProcessor mergeProcessor)
        {
            _gridView = gridView;
            _mergeProcessor = mergeProcessor;
        }

        public void HighlightAllPotentialCombinations(List<ItemData> allItems)
        {
            StopHighlight();
            _highlightedCells = new List<ICellView>(allItems.Count);
            var cellsForMerge = new List<Vector2Int>(allItems.Count);
            foreach (var item in allItems)
            {
                var coords = _mergeProcessor.GetCellsForPotentialMerge(allItems, item);
                if (coords != null)
                {
                    cellsForMerge.AddRange(coords);
                    // var msg = $"Item at: {item.core.ItemDataStr()}\n";
                    // foreach (var cc in coords)
                    //     msg += $"{cc}, ";
                    // CLog.LogWhite(msg);
                }
            }
            if (cellsForMerge.Count == 0)
            {
                // CLog.Log($"[{nameof(MergeGridHighlighter)}] No cells to highlight");
                return;
            }
            foreach (var cell in cellsForMerge)
            {
                var view = _gridView.GetCell(cell.x, cell.y);
                view.SetHighlightForMerge(true, 1);
                _highlightedCells.Add(view);
            }
        }
        
        public void HighlightForSpecificItem(List<ItemData> allItems, ItemData srcItem)
        {
            StopHighlight();
            _highlightedCells = new List<ICellView>(allItems.Count);
            var cellsForMerge = _mergeProcessor.GetCellsForPotentialMerge(allItems, srcItem);
            if (cellsForMerge == null || cellsForMerge.Count == 0)
                return;
            foreach (var cell in cellsForMerge)
            {
                var view = _gridView.GetCell(cell.x, cell.y);
                view.SetHighlightForMerge(true, 2);
                _highlightedCells.Add(view);
            }
        }

        public void StopHighlight()
        {
            if (_highlightedCells == null || _highlightedCells.Count == 0)
                return;
            foreach (var view in _highlightedCells)
                view.SetHighlightForMerge(false, 1);
        }
    }
}