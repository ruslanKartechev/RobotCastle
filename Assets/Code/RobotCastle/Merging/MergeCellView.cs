using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeCellView : MonoBehaviour, ICellView
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private IItemView _item;
        public Cell cell { get; set; }
        
        public IItemView item
        {
            get => _item;
            set
            {
                if (value == null)
                {
                    _item = null;
                    cell.SetEmpty();
                    // CLog.LogRed($"{cell.x} {cell.y} SET EMPTY");
                }
                else
                {
                    _item = value;
                    cell.SetItem(value.Data); 
                }
            } 
        }

        public void OnPicked()
        {
        }

        public void OnPut()
        {
        }

        public void OnDroppedBack()
        {
        }

        public void SetHighlightForMerge(bool on)
        {
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            var mat = on ? db.cellHighlightedMaterial : db.cellDefaultMaterial;
            _meshRenderer.sharedMaterial = mat;
        }

        public Transform ItemPoint => transform;
    }
}