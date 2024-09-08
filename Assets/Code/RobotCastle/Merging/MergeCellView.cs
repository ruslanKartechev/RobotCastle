using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeCellView : MonoBehaviour, ICellView
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Transform _itemPoint;
        private IItemView _item;
        private CellHighlight _highlight;
        
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

        public void OnPicked()
        { }

        public void OnPut()
        { }

        public void OnDroppedBack()
        { }

        public void SetHighlightForMerge(bool on, int type)
        {
            if (on && _highlight == null)
            {
                switch (type)
                {
                    case 1:
                        _highlight = ServiceLocator.Get<IMergeCellHighlightPool>().GetOneType1();
                        break;
                    case 2:
                        _highlight = ServiceLocator.Get<IMergeCellHighlightPool>().GetOneType2();
                        break;
                }
                _highlight.HighlightType = type;
                _highlight.ShowAt(transform.position);
            }
            else if (!on && _highlight != null)
            {
                _highlight.Hide();
                switch (_highlight.HighlightType)
                {
                    case 1:
                        ServiceLocator.Get<IMergeCellHighlightPool>().ReturnType1(_highlight);
                        break;
                    case 2:
                        ServiceLocator.Get<IMergeCellHighlightPool>().ReturnType2(_highlight);
                        break;
                }
                _highlight = null;
            }
        }

        public void SetHighlightForAttack(bool on)
        {
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            var mat = on ? db.cellHighlightedForAttackMaterial : db.cellDefaultMaterial;
            _meshRenderer.sharedMaterial = mat;
        }
    }
}