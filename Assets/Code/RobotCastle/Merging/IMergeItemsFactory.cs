using System.Collections.Generic;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IMergeItemsFactory
    {
        IItemView SpawnItemOnCell(ICellView pivotCell, ItemData item);
        List<IItemView> SpawnItems(List<CoreItemData> itemData);
        
        /// <summary>
        /// Only instantiates the prefab. Does not do any initialization,
        /// </summary>
        GameObject SpawnItem(ItemData itemData);
    }
}