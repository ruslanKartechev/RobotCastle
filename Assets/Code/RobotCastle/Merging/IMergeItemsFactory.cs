using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.Merging
{
    public interface IMergeItemsFactory
    {
        IItemView SpawnItemOnCell(ICellView pivotCell, ItemData item);
        List<IItemView> SpawnItems(List<CoreItemData> itemData);
        

    }
}