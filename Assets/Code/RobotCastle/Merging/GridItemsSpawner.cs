using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class GridItemsSpawner : MonoBehaviour, IGridItemsSpawner
    {
        [SerializeField] private Transform _unitsParent;
        
        public void SpawnItemsForGrid(IGridView gridView, MergeGrid saves)
        {
            var db = ServiceLocator.Get<ViewDataBaseContainer>().DataBase;
            for (var y = 0; y < saves.RowsCount; y++)
            {
                for (var x = 0; x < saves.rows[y].cells.Count; x++)
                {
                    var cell = saves.rows[y].cells[x];
                    if (cell.isOccupied)
                    {
                        var itemData = cell.currentItem;
                        var pivotCell = saves.rows[itemData.pivotY].cells[itemData.pivotX];
                        var pivotCellView = gridView.GetCell(pivotCell.x, pivotCell.y);
                        
                        var prefab = db.GetMergePrefab(itemData.core.id);
                        if (prefab == null)
                        {
                            CLog.LogError($"prefab is null!!! {itemData.core.id}");
                        }
                        var instance= SleepDev.MiscUtils.Spawn(prefab, _unitsParent);
                        instance.transform.position = pivotCellView.ItemPoint.position;
                        var itemView = instance.GetComponent<IItemView>();
                        itemView.Data = itemData;
                        pivotCellView.item = itemView;
                        
                        itemData.pivotX = pivotCell.x;
                        itemData.pivotY = pivotCell.y;
                        // extend for case of multi cell items!
                    }
                }
            }
        }

        public void SpawnItemOnCell(ICellView cellView, ItemData itemData)
        {
            var db = ServiceLocator.Get<ViewDataBaseContainer>();
            GameObject prefab = null;
            
            if(itemData.core.type == MergeConstants.TypeItems) 
                prefab = db.DataBase.GetMergePrefabAtLevel(itemData.core.id, itemData.core.level);
            else
                prefab = db.DataBase.GetMergePrefab(itemData.core.id);
            
            var instance= SleepDev.MiscUtils.Spawn(prefab, transform);
            instance.transform.position = cellView.ItemPoint.position;
            var itemView = instance.GetComponent<IItemView>();
            itemData.pivotX = cellView.cell.x;
            itemData.pivotY = cellView.cell.y;
            itemView.Data = itemData;
            cellView.item = itemView;
            
            // extend for case of multi cell items!
        }

        public void SpawnItemOnCell(ICellView cellView, string itemId)
        {
            var db = ServiceLocator.Get<ViewDataBaseContainer>();
            var prefab = db.DataBase.GetMergePrefab(itemId);
            var instance= SleepDev.MiscUtils.Spawn(prefab, transform);
            instance.transform.position = cellView.ItemPoint.position;
            var itemView = instance.GetComponent<IItemView>();
            itemView.Data = new ItemData(0, itemId);
            cellView.item = itemView;
            // extend for case of multi cell items!
        }
    }
}