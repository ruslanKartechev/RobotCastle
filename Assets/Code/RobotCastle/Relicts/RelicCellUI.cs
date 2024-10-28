using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Relicts
{
    public class RelicCellUI : MonoBehaviour, IPoolItem
    {
        public RelicItemUI item { get; set; }
        
        public bool isEmpty => item == null;

        public void SetAndParentItem(RelicItemUI item)
        {
            this.item = item;
            item.transform.SetParent(transform, false);
            item.transform.localPosition = Vector3.zero;
        }
  
        public GameObject GetGameObject() => gameObject;

        public bool IsAvailable { get; set; } = true;

        public string PoolId { get; set; }
        
        public void PoolHide() => gameObject.SetActive(false);

        public void PoolShow() => gameObject.SetActive(true);

    }
}