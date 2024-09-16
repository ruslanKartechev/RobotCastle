namespace RobotCastle.Merging
{
    public partial class MergeController
    {
        private class DraggedItem
        {
            public ICellView originalCellView;
            public IItemView itemView;
            public ICellView underCell;
            public MergeUnitRangeHighlighter rangeHighlighter; 
            
            public DraggedItem(ICellView cell, IItemView item, IGridView grid)
            {
                originalCellView = cell;
                itemView = item;
                underCell = null;
                if(item.itemData.core.type == MergeConstants.TypeUnits)
                    rangeHighlighter = new MergeUnitRangeHighlighter(item.Transform.gameObject, grid);
            }

            public void OnPut()
            {
                if (rangeHighlighter != null)
                {
                    rangeHighlighter.Clear();
                    rangeHighlighter = null;                        
                }
            }
            
            public void PutBack() // On Put is called is after this one!
            {
                itemView.Transform.position = originalCellView.ItemPoint.position;
                itemView.Transform.rotation = originalCellView.ItemPoint.rotation;
                originalCellView.OnDroppedBack();
                itemView.OnDroppedBack();
                itemView.MoveToPoint(originalCellView.ItemPoint, MergeConstants.MergeItemPutAnimationTime);
            }

            public void SetUnderCell(ICellView cell)
            {
                if (underCell == cell)
                    return;
                if (cell == null)
                {
                    underCell = null;
                    return;
                }
                underCell = cell;
                itemView.Rotate(cell.ItemPoint.rotation, MergeConstants.MergeItemPutAnimationTime);
                if (rangeHighlighter != null)
                {
                    rangeHighlighter.UpdateForUnderCell(underCell.cell.Coord);
                    rangeHighlighter.ShowUnderCell(underCell.cell.Coord);             
                }
            }
            
        }
    }
}