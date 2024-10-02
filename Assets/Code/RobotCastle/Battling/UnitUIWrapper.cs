using RobotCastle.Core;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitUIWrapper : MonoBehaviour, IPoolItem
    {
        public object ui;
        public UIWorldPositionTracker tracker;

        public void ReturnBack()
        {
            var panel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            panel.ReturnToPool(this);
        }

        public string PoolId { get; set; }
        public GameObject GetGameObject() => gameObject;
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(true);
    }
}