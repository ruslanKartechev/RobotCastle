using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobotCastle.Relics
{
    public class RelicsUIPanel : MonoBehaviour, IScreenUI
    {
        [SerializeField] private Pool _relicsPool;
        [SerializeField] private Pool _cellsPool;
        [SerializeField] private int _minRowsCount = 20;
        [SerializeField] private float _cellsHeight = 300;
        [SerializeField] private int _perRowCount = 5;
        [SerializeField] private RectTransform _parentsRect;
        [SerializeField] private RectTransform _animatedRect;
        [SerializeField] private Vector2 _rectHeight;
        [SerializeField] private float _animationTime;
        [SerializeField] private RectTransform _rectArrowSort;
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private GameObject _inventory;
        [SerializeField] private RelicShortDescriptionUI _description;
        [SerializeField] private EquippedRelicsPanel _equippedPanel;
        [SerializeField] private float _shortClickMaxTime = .15f;
        [Space(10)]
        [SerializeField] private MyButton _closeBtn;
        [SerializeField] private MyButton _sortBtn;
        private List<RelicItemUI> _ownedRelics;
        private List<RelicCellUI> _cells;
        private float _clickTime;
        private bool _didInit;
        private bool _sortAscending;
        private bool _isDragging;
        private RelicCellUI _draggedOriginalCell;
        private RelicItemUI _draggedItem;
        private Coroutine _dragging;
        private bool _inputSub;
        private bool _descriptionShown;


        public void Show()
        {
            if (!_inputSub)
                SubInput(true);
            _inventory.SetActive(true);
            gameObject.SetActive(true);
            if (!_didInit)
            {
                _didInit = true;
                _cellsPool.Init();
                _relicsPool.Init();
            }
            var saves = DataHelpers.GetPlayerData().relics;
            var db = ServiceLocator.Get<RelicsDataBase>();
            var totalAmount = saves.allRelics.Count;
            
            var cellsCount = _minRowsCount * _perRowCount;
            if (totalAmount > cellsCount)
                cellsCount = (totalAmount % _perRowCount + 1) * _perRowCount;
            if (_cells == null)
                _cells = new List<RelicCellUI>(cellsCount);
            else
                ClearAllCells();
            for (var i = _cells.Count; i < cellsCount; i++)
            {
                var cell = _cellsPool.GetOne() as RelicCellUI;
                cell.PoolShow();
                _cells.Add(cell);
            }

            var sizeDelta = _parentsRect.sizeDelta;
            sizeDelta.y = cellsCount % _perRowCount * _cellsHeight + 50;
            _parentsRect.sizeDelta = sizeDelta;

            if(_ownedRelics == null)
                _ownedRelics = new List<RelicItemUI>(totalAmount);
            else
            {
                foreach (var rel in _ownedRelics)
                    _relicsPool.Return(rel);
                _ownedRelics.Clear();
            }
            foreach (var save in saves.allRelics)
            {
                var data = db.relicData[save.core.id];
                var icon = Resources.Load<Sprite>(data.icon);
                
                var ui = _relicsPool.GetOne() as RelicItemUI;
                ui.SetDataAndIcon(data, save, icon);
                ui.PoolShow();
                _ownedRelics.Add(ui);
            }

            _closeBtn.AddMainCallback(Close);
            _sortBtn.AddMainCallback(ChangeSortOrder);
            
            SetBtnStateToOrder();
            SortItems();
            AnimateIn();
        }

        public void Close()
        {
            if (_inputSub)
                SubInput(false);
            var size = _animatedRect.sizeDelta;
            size.y = _rectHeight.x;
            _animatedRect.DOSizeDelta(size, _animationTime).OnComplete(() =>
            {
                _inventory.SetActive(false);
            });
            DropResetDragging();
            ServiceLocator.Get<IUIManager>().OnClosed(UIConstants.UIRelics);
        }
        
        private void Start()
        {
            ServiceLocator.Get<IUIManager>().AddAsShown(UIConstants.UIRelics, this);
            _inventory.SetActive(false);
        }

        private void OnEnable()
        {
            var inp = ServiceLocator.Get<GameInput>();
            inp.OnDownIgnoreUI += OnDownClick;
            inp.OnUpIgnoreUI += OnUpClick;
        }

        private void OnDisable()
        {
            var inp = ServiceLocator.Get<GameInput>();
            inp.OnDownIgnoreUI -= OnDownClick;
            inp.OnUpIgnoreUI -= OnUpClick;
        }

        private void OnDownClick(Vector3 pos)
        {
            _clickTime = Time.time;
            HideDescriptionIfOpened();
        }

        private void OnUpClick(Vector3 pos)
        {
            if (Time.time - _clickTime < _shortClickMaxTime)
            {
                var cell = RaycastForCell(pos);
                if (cell == null) return;
                if (cell.isEmpty || !cell.IsAvailable) return;
                _descriptionShown = true;
                _description.Show(cell.item);
                return;
            }
            HideDescriptionIfOpened();
        }
        
        private void HideDescriptionIfOpened()
        {
            if (_descriptionShown)
            {
                _descriptionShown = false;
                _description.Hide();
            }
        }

        private void ChangeSortOrder()
        {
            _sortAscending = !_sortAscending;
            SetBtnStateToOrder();
            SortItems();
        }

        private void ClearAllCells()
        {
            foreach (var cell in _cells)
                cell.item = null;
        }

        private void SortItems()
        {
            ClearAllCells();
            var cellIndex = _cells.Count - 1;
            if (_sortAscending)
            {
                _ownedRelics.Sort((a, b) => a.relicData.core.tier.CompareTo(b.relicData.core.tier));            
            }
            else
            {
                _ownedRelics.Sort((b, a) => a.relicData.core.tier.CompareTo(b.relicData.core.tier));
            }
            foreach (var ui in _ownedRelics)
            {
                var cell = _cells[cellIndex];
                cell.SetAndParentItem(ui);
                ui.PoolShow();
                cellIndex--;
            }
        }

        private void SetBtnStateToOrder()
        {
            var angle = _sortAscending ? 180 : 0;
            var eulers = _rectArrowSort.eulerAngles;
            eulers.z = angle;
            _rectArrowSort.eulerAngles = eulers;
        }

        private void AnimateIn()
        {
            var sizeDelta = _animatedRect.sizeDelta;
            sizeDelta.y = _rectHeight.x;
            _animatedRect.sizeDelta = sizeDelta;
            sizeDelta.y = _rectHeight.y;
            _animatedRect.DOSizeDelta(sizeDelta, _animationTime);
        }

        private void SubInput(bool sub)
        {
            _inputSub = sub;
            if (sub)
            {
                ServiceLocator.Get<GameInput>().OnUpMain += OnUp;
                ServiceLocator.Get<GameInput>().OnDownLongIgnoreUIClick += OnLongClick;
            }
            else
            {
                ServiceLocator.Get<GameInput>().OnUpMain -= OnUp;
                ServiceLocator.Get<GameInput>().OnDownLongIgnoreUIClick -= OnLongClick;
            }
        }
        
        
        private void DropResetDragging()
        {
            _isDragging = false;
            _draggedOriginalCell = null;
            _draggedItem = null;
            if(_dragging != null)
                StopCoroutine(_dragging);
        }

        private void OnUp(Vector3 pos)
        {
            if (!_isDragging) return;
            if(_dragging != null)
                StopCoroutine(_dragging);
            var cell = RaycastForCell(pos);
            if (cell == null)
            {
                DropBack();
                return;
            }

            if (!cell.IsAvailable)
            {
                DropBack();
                return;
            }
            if (cell.TryGetComponent<EquippedRelicUI>(out var equippedCell) == false)
            {
                DropBack();
                return;
            }
            if (!equippedCell.IsUnlocked)
            {
                DropBack();
                return;
            }
            if (_equippedPanel.HasRelicWithId(_draggedItem.relicData.core.id))
            {
                DropBack();
                return;
            }

            var icon = _draggedItem.GetIcon();
            if (equippedCell.relicData != null)
            {
                var prevEquipped = equippedCell.relicData.core;
                var prev = _ownedRelics.Find(t => t.relicData.core == prevEquipped);
                if (prev != null)
                {
                    prev.relicSave.isEquipped = false;
                    prev.SetEquipped(false);
                }
                else
                {
                    Debug.LogError("Error! Cannot find ui element of the previously equipped relic");
                }
            }
            equippedCell.SetDataAndIcon(_draggedItem.relicData, _draggedItem.relicSave, icon);
            
            _draggedItem.SetEquipped(true);
            DropBack();
            
            void DropBack()
            {
                _draggedOriginalCell.SetAndParentItem(_draggedItem);
                DropResetDragging();
            }
        }

        private void OnLongClick(Vector3 pos)
        {
            HideDescriptionIfOpened();
            var cell = RaycastForCell(pos);
            if (cell == null) return;
            if (cell.isEmpty || !cell.IsAvailable) return;
            
            _isDragging = true;
            _draggedOriginalCell = cell;
            _draggedItem = cell.item;
            _draggedItem.transform.SetParent(transform);
            if(_dragging != null)
                StopCoroutine(_dragging);
            StartCoroutine(Dragging());
            _scrollView.vertical = false;
        }

        private IEnumerator Dragging()
        {
            while (_isDragging)
            {
                var pos = Input.mousePosition;
                _draggedItem.transform.position = pos;
                
                yield return null;
            }
        }
        
        private RelicCellUI RaycastForCell(Vector3 pos)
        {
            var dd = new PointerEventData(EventSystem.current);
            dd.position = pos;
            var output = new List<RaycastResult>(2);
            _raycaster.Raycast(dd, output);
            if (output.Count == 0)
                return null;
            foreach (var res in output)
            {
                if (res.gameObject.TryGetComponent<RelicCellUI>(out var cell))
                {
                    return cell;
                }
            }
            return null;
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void On()
        {
            gameObject.SetActive(true);
        }
    }
}