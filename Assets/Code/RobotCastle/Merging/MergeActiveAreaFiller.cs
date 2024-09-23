using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeActiveAreaFiller
    {
        private IGridView _grid;
        private IGridSectionsController _sectionsController;

        public MergeActiveAreaFiller(IGridView grid, IGridSectionsController sectionsController)
        {
            _grid = grid;
            _sectionsController = sectionsController;
        }

        public void Fill()
        {
            if (_sectionsController.CanPutMoreIntoActiveZone() == false)
                return;
            var items = _sectionsController.GetAllItemViewsInMergeArea();
            var heroes = new List<IItemView>(items.Count);
            foreach (var vv in items)
            {
                if(vv.itemData.core.type == MergeConstants.TypeUnits)
                    heroes.Add(vv);
            }
            if (heroes.Count == 0)
                return;
            CLog.LogRed($"Selecting from: {heroes.Count}");
            heroes.Sort((a, b) =>
            {
                var core1 = a.itemData.core;
                var core2 = b.itemData.core;
                if (core1.type == core2.type)
                {
                    if (core1.id == core2.id)
                        return core2.level.CompareTo(core1.level);
                    else
                        return String.Compare(core2.id, core1.id, StringComparison.Ordinal);
                }
                return 0;
            });
            var ind = 0;
            while (ind < heroes.Count && _sectionsController.CanPutMoreIntoActiveZone())
            {
                var originalCoord = new Vector2Int(heroes[ind].itemData.pivotX, heroes[ind].itemData.pivotY);
                var coord = _sectionsController.GetCoordinateForClosestCellInActiveZone(originalCoord);
                MergeFunctions.ClearCell(_grid, heroes[ind]);
                MergeFunctions.PutItemToCell(heroes[ind], _grid.GetCell(coord.x, coord.y));
                _sectionsController.OnItemPut(heroes[ind].itemData);
                ind++;
            }
        }
    }
}