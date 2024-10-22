using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroUIWrapper : MonoBehaviour, IPoolItem
    {
        public Component ui;
        public UIWorldPositionTracker tracker;

        public void ReturnBack()
        {
            var panel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            panel.ReturnToPool(this);
        }

        public string PoolId
        {
            get => _poolId; set => _poolId = value;
        }
        public GameObject GetGameObject() => gameObject;
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(true);
        
        private string _poolId;

    }
}