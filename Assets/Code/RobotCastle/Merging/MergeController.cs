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
        

        private MergeGrid _grid;
        private IMergeProcessor _processor;
        private IMergeItemsFactory _itemsSpawner;
        private IGridView _gridView;
        private LayerMask _layerMask;
        private Vector3 _offset;
        private IGridSectionsController _sectionsController;
        private Camera _camera;
        private DraggedItem _draggedItem;
        private MergePutResult _lastPutResult;
        private ICellView _lastPutCell;
        private bool _isProcessingPut;
        
        
        public MergeController(MergeGrid grid,
            IMergeProcessor processor, 
            IMergeItemsFactory itemsSpawner,
            IGridSectionsController availabilityController,
            IGridView gridView)
        {
            _grid = grid;
            _processor = processor;
            _itemsSpawner = itemsSpawner;
            _sectionsController = availabilityController;
            _gridView = gridView;
            _camera = Camera.main;
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            _layerMask = db.cellsMask;
            _offset = db.draggingOffset;
        }

        public void OnDown(Vector3 screenPosition)
        {
            // CLog.Log($"[{nameof(MergeController)}] On Down");
            if (_isProcessingPut)
                return;
            var cell = RaycastForCellView(screenPosition);
            if (cell == null)
            {
                PutDown();
                return;
            }
            if (!cell.cell.isOccupied)
            {
                PutDown();
                return;
            }
            var item = cell.itemView;
            _draggedItem = new DraggedItem(cell, item, _gridView);
            cell.OnPicked();
            item.OnPicked();
            _draggedItem.SetUnderCell(cell);
            OnItemPicked?.Invoke(item.itemData);
            // CLog.Log($"Raycast: {cell.cell.x}, {cell.cell.y}. Picked {item.Data.GetStr()}");
        }

        public void OnUp(Vector3 screenPosition)
        {
            // CLog.Log($"[{nameof(MergeController)}] On Up");
            if (_draggedItem == null)
                return;
            if (_isProcessingPut)
            {
                CLog.LogRed("_isProcessingPut !!!!!!!!!!!");
                return;
            }
            _isProcessingPut = true;
            var putResult = MergePutResult.MissedCell;
            // var (didHitCell, cellView) = TryHitCellToPut(screenPosition);
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
                    if (!_sectionsController.IsCellAllowed(cellView.cell.x, cellView.cell.y, _draggedItem.itemView.itemData))
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
                    _processor.TryMerge(_draggedItem.itemView, cellView.itemView, _gridView, MergeCallback, out var oneIntoTwo);
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
        
        public bool GetFreeCellForNewHero(out ICellView cellView)
        {
            var hasFree = _sectionsController.GetFreeCell(_grid, out var coord);
            if (!hasFree)
            {
                cellView = null;
                return false;
            }
            cellView = _gridView.GetCell(coord.x, coord.y);
            return true;
        }
        
        private void MergeCallback(EMergeResult mergeResult, bool oneIntoTwo)
        {
            CLog.LogBlue($"MergeCallback {mergeResult}");
            var putResult = MergePutResult.Merged;
            switch (mergeResult)
            {
                case EMergeResult.NoMerge:
                    if(!TrySwap(_draggedItem.originalCellView, _lastPutCell))
                        _draggedItem.PutBack();
                    putResult = MergePutResult.MergeFailed;
                    break;
                case EMergeResult.MergedIntoNew:
                    putResult = MergePutResult.Merged;
                    break;
                case EMergeResult.MergedOneIntoAnother:
                    putResult = MergePutResult.Merged;
                    break;
            }
            _lastPutResult = putResult;
            UpdateGridAndNullDragged();
        }
        
        private void PutDown()
        {
            if(_draggedItem != null)
                _draggedItem.OnPut();
            _draggedItem = null;
        }
        
        private void UpdateGridAndNullDragged()
        {
            _isProcessingPut = false;
            _sectionsController.OnItemPut(_draggedItem.itemView.itemData);
            PutDown();
            CLog.Log($"[{nameof(MergeController)}] Put Result: {_lastPutResult.ToString()}");
            OnPutItem?.Invoke(_lastPutResult);
        }

        private bool TrySwap(ICellView cell1, ICellView cell2)
        {
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
        
        private (bool, ICellView) TryHitCellToPut(Vector3 screenPosition)
        {
            var didHitCell = false;
            var scrPos = _camera.WorldToScreenPoint(_draggedItem.itemView.Transform.position);
            var cellView = RaycastForCellView(scrPos);
            if (cellView == null)
            {
                cellView = RaycastForCellView(screenPosition);
                if (cellView == null)
                    _draggedItem.PutBack();             
                else
                    didHitCell = true;
            }
            else
                didHitCell = true;

            return (didHitCell, cellView);
        }
    }
}