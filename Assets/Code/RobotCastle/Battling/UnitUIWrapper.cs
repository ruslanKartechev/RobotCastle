using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitUIWrapper : MonoBehaviour, IPoolItem
    {
        public Component ui;
        public UIWorldPositionTracker tracker;
        private string _poolId;

        public void ReturnBack()
        {
            var panel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            panel.ReturnToPool(this);
        }

        public string PoolId
        {
            get => _poolId; set
            {
                Debug.Log($"=================== SET ID: __{value}__ for {gameObject.name}");
                _poolId = value;
            } }
        public GameObject GetGameObject() => gameObject;
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(true);
    }
}