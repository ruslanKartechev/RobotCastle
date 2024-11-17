using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeCellView : MonoBehaviour, ICellView
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Transform _itemPoint;
        [SerializeField] private Transform _lowerPoint;
        private IItemView _item;
        private CellHighlight _highlight;

        public Transform LowerPoint => _lowerPoint;

        public int GridId { get; set; }
        
        public Cell cell { get; set; }
        
        public IItemView itemView
        {
            get => _item;
            set
            {
                if (value == null)
                {
                    _item = null;
                    cell.SetEmpty();
                }
                else
                {
                    _item = value;
                    cell.SetItem(value.itemData); 
                }
            } 
        }

        public void HighlightAsUnderCell(bool on)
        {
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            var mat = on ? db.cellHighlightedMaterial : db.cellDefaultMaterial;
            _meshRenderer.sharedMaterial = mat;
        }

        public Transform ItemPoint => _itemPoint;
        public Vector3 WorldPosition => transform.position;
        public Quaternion WorldRotation => transform.rotation;

        public void OnPicked() { }

        public void OnPut() { }

        public void OnDroppedBack() { }

        public void SetHighlightForMerge(bool on, int type)
        {
            if (on && _highlight == null)
            {
                switch (type)
                {
                    case 1:
                        _highlight = ServiceLocator.Get<ISimplePoolsManager>().GetOne(ObjectPoolConstants.MergeDefaultFxId) as CellHighlight;
                        break;
                    case 2:
                        _highlight = ServiceLocator.Get<ISimplePoolsManager>().GetOne(ObjectPoolConstants.MergePickedFxId) as CellHighlight;;
                        break;
                }
                _highlight.HighlightType = type;
                _highlight.ShowAt(transform.position);
            }
            else if (!on && _highlight != null)
            {
                ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(_highlight);
                _highlight = null;
            }
        }

    }
}