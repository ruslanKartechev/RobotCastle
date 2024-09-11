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
                        itemView.itemData = itemData;
                        MergeFunctions.PutItemToCell(itemView, pivotCellView);
                        // extend for case of multi cell items!
                    }
                }
            }
        }

        public IItemView SpawnItemOnCell(ICellView pivotCell, ItemData itemData)
        {
            var db = ServiceLocator.Get<ViewDataBaseContainer>();
            GameObject prefab = null;
            GameObject instance = null;
            switch (itemData.core.type)
            {
                case MergeConstants.TypeItems:
                    prefab = db.DataBase.GetMergePrefabAtLevel(itemData.core.id, itemData.core.level);
                    break;
                case MergeConstants.TypeUnits:
                    prefab = db.DataBase.GetMergePrefab(itemData.core.id);
                    break;
                default:
                    CLog.LogRed($"[GridItemsSpawner] {itemData.core.type} Unknown type");
                    break;
            }
            instance = SleepDev.MiscUtils.Spawn(prefab, transform);
            instance.transform.position = pivotCell.ItemPoint.position;
            var itemView = instance.GetComponent<IItemView>();
            itemView.InitView(itemData);
            MergeFunctions.PutItemToCell(itemView, pivotCell);
            // extend for case of multi cell items!
            return itemView;
        }

        public IItemView SpawnItemOnCell(ICellView pivotCell, string itemId)
        {
            var db = ServiceLocator.Get<ViewDataBaseContainer>();
            var prefab = db.DataBase.GetMergePrefab(itemId);
            var instance= SleepDev.MiscUtils.Spawn(prefab, transform);
            instance.transform.position = pivotCell.ItemPoint.position;
            var itemView = instance.GetComponent<IItemView>();
            itemView.itemData = new ItemData(0, itemId);
            pivotCell.itemView = itemView;
            MergeFunctions.PutItemToCell(itemView, pivotCell);

            // extend for case of multi cell items!
            return itemView;
        }
    }
}