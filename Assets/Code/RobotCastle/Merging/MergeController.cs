using System;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public partial class MergeController
    {
        private const float RaycastMaxDistance = 100;

        public event Action<MergePutResult> OnPutItem; 
        public event Action<ItemData> OnItemPicked;

        public bool useHighlight = true;
        
        private IMergeProcessor _processor;
        private IGridView _gridView;
        private IGridSectionsController _sectionsController;
        private ISwapAllowedCheck _swapAllowedCheck;
        private IMergeItemsContainer _container;
        private ICellView _lastPutCell;
        private LayerMask _layerMask;
        private Camera _camera;
        private DraggedItem _draggedItem;
        private MergePutResult _lastPutResult;
        private bool _isProcessingPut;

        public IItemView DraggedItemView => _draggedItem.itemView;
        
        public MergeController(IMergeProcessor processor, 
            IGridSectionsController sectionsController,
            IGridView gridView,
            ISwapAllowedCheck swapAllowedCheck,
            IMergeItemsContainer container)
        {
            _swapAllowedCheck = swapAllowedCheck;
            _processor = processor;
            _sectionsController = sectionsController;
            _gridView = gridView;
            _container = container;
            _camera = Camera.main;
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            _layerMask = db.cellsMask;
        }

        public void MergeIfPossible(Vector3 screenPosition)
        {
            if (_isProcessingPut)
                return;
            var cell = RaycastForCellView(screenPosition);
            if (cell == null 
                || cell.GridId != _gridView.GridId
                || !cell.cell.isOccupied)
            {
                NullDragged();
                return;
            }
            _isProcessingPut  = true;
            var item = cell.itemView;
            _processor.TryMergeWithAny(item, _gridView, AutoMergeCallback);

        }
        
        public void OnDown(Vector3 screenPosition)
        {
            // CLog.Log($"[{nameof(MergeController)}] On Down");
            if (_isProcessingPut)
                return;
            var cell = RaycastForCellView(screenPosition);
            if (cell == null 
                || cell.GridId != _gridView.GridId
                || !cell.cell.isOccupied)
            {
                NullDragged();
                return;
            }
       
            var item = cell.itemView;
            _draggedItem = new DraggedItem(cell, item, _gridView, useHighlight);
            cell.OnPicked();
            item.OnPicked();
            _draggedItem.SetUnderCell(cell);
            OnItemPicked?.Invoke(item.itemData);
        }

        /// <summary>
        /// </summary>
        /// <returns>Level of the dropped item</returns>
        public int DropToReturnItem()
        {
            if (_draggedItem == null || _draggedItem.itemView == null)
                return -1;
            var lvl = _draggedItem.itemView.itemData.core.level;
            _draggedItem.originalCellView.itemView = null;
            _processor.BreakItemToReturn(_draggedItem.itemView);
            NullDragged();
            _isProcessingPut = false;
            return lvl;
        }

        public void OnUp(Vector3 _)
        {
            // CLog.Log($"[{nameof(MergeController)}] On Up");
            if (_draggedItem == null)
                return;
            if (_isProcessingPut)
                return;
            _isProcessingPut = true;
            var putResult = MergePutResult.MissedCell;
            var cellView = _draggedItem.underCell;
            if (cellView == null)
                cellView = GetClosestCellTo(_draggedItem.itemView.Transform.position);
            if(cellView != null)
            {
                _lastPutCell = cellView;
                if (cellView == _draggedItem.originalCellView)
                {
                    putResult = MergePutResult.PutToSameCell;
                    _draggedItem.PutBack();
                }
                else if (!cellView.cell.isOccupied) // cell is free
                {
                    if (!_sectionsController.IsCellAllowed(new Vector2Int(cellView.cell.x, cellView.cell.y), _draggedItem.itemView.itemData))
                    {
                        putResult = MergePutResult.CellNotAllowed;
                        _draggedItem.PutBack();
                    }
                    else
                    {
                        _draggedItem.originalCellView.itemView = null;
                        MergeFunctions.PutToCellAnimated(_draggedItem.itemView, cellView);
                        putResult = MergePutResult.PutToEmptyCell;              
                    }
                }
                else // cell is occupied
                {
                    _processor.TryMerge(_draggedItem.itemView, cellView.itemView, _gridView, DraggedMergeCallback);
                    return;
                }
            }
            else
            {
                _draggedItem.PutBack();             
            }
            _lastPutResult = putResult;
            UpdateGridAndNullDragged();
        }

        public void OnMove(Vector3 screenPosition)
        {
            if (_draggedItem == null || _isProcessingPut)
                return;
            if (Physics.Raycast(_camera.ScreenPointToRay(screenPosition), out var hitInfo, 100, _layerMask))
            {
                _draggedItem.itemView.Transform.position = hitInfo.point + ServiceLocator.Get<MergeGridViewDataBase>().draggingOffset;
            }
            // var ray = new Ray(_draggedItem.itemView.Transform.position + Vector3.up, Vector3.down);
            var pos = _draggedItem.itemView.Transform.position;
            var scrPoint = _camera.WorldToScreenPoint(pos);
            var underCell = (ICellView)null;
            if (Physics.Raycast(_camera.ScreenPointToRay(scrPoint), out var hitInfo2, 100, _layerMask))
            {
                underCell = hitInfo2.collider.gameObject.GetComponent<ICellView>();
                if (underCell == null)
                    underCell = GetClosestCellTo(pos);
            }
            else
                underCell = GetClosestCellTo(pos);
            _draggedItem.SetUnderCell(underCell);
        }

        private ICellView GetClosestCellTo(Vector3 worldPos)
        {
            var d2 = float.MaxValue;
            var minD2 = float.MaxValue;
            ICellView resultCell = null;
            foreach (var cellView in _gridView.Grid)
            {
                var vec = worldPos - cellView.ItemPoint.position;
                d2 = vec.XZDistance2();
                if (d2 < minD2)
                {
                    minD2 = d2;
                    resultCell = cellView;
                }
            }
            return resultCell;
        }

        
        private void AutoMergeCallback(EMergeResult mergeResult, bool oneIntoTwo)
        {
            _isProcessingPut = false;
            _sectionsController.OnGridUpdated();
            var result = MergePutResult.Merged;
            if (mergeResult == EMergeResult.NoMerge)
                result = MergePutResult.MergeFailed;
            OnPutItem?.Invoke(result);
        }
        
        private void DraggedMergeCallback(EMergeResult mergeResult, bool oneIntoTwo)
        {
            var putResult = MergePutResult.Merged;
            switch (mergeResult)
            {
                case EMergeResult.NoMerge:
                    if (TrySwap(_draggedItem.originalCellView, _lastPutCell))
                    {
                        _sectionsController.OnItemPut(_draggedItem.originalCellView.itemView.itemData);
                        _lastPutCell.itemView.OnPut();
                    }
                    else
                        _draggedItem.PutBack();
                    putResult = MergePutResult.MergeFailed;
                    break;
            }
            _lastPutResult = putResult;
            UpdateGridAndNullDragged();
        }
        
        private void NullDragged()
        {
            if(_draggedItem != null)
                _draggedItem.OnDraggingEnd();
            _draggedItem = null;
        }
        
        private void UpdateGridAndNullDragged()
        {
            _isProcessingPut = false;
            if(_draggedItem.itemView != null)
                _sectionsController.OnItemPut(_draggedItem.itemView.itemData);
            NullDragged();
            CLog.Log($"[{nameof(MergeController)}] Put Result: {_lastPutResult.ToString()}");
            OnPutItem?.Invoke(_lastPutResult);
        }

        private bool TrySwap(ICellView cell1, ICellView cell2)
        {
            if(!_swapAllowedCheck.IsSwapAllowed(cell1, cell2))
                return false;
            var item1 = cell1.itemView;
            var item2 = cell2.itemView;
            cell1.itemView = item2;
            cell2.itemView = item1;
            MergeFunctions.SetItemGridAndWorldPosition(item1, cell2);
            MergeFunctions.SetItemGridAndWorldPosition(item2, cell1);
            return true;
        }

        private ICellView RaycastForCellView(Vector3 screenPos)
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(screenPos), out var hitInfo, RaycastMaxDistance, _layerMask))
            {
                return hitInfo.collider.gameObject.GetComponent<ICellView>();
            }
            return null;
        }

    
    }
}