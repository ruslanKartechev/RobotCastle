using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class StunnedFX : MonoBehaviour, IPoolItem
    {

        public string PoolId { get; set; }
        public GameObject GetGameObject() => gameObject;
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(true);

    }
}