using System.Threading.Tasks;
using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class DeathParticles : MonoBehaviour, IPoolItem
    {
        private const float HideDelay = 1.5f * 1000;
        
        public async void Show(Vector3 worldPos)
        {
            transform.position = worldPos;
            gameObject.SetActive(true);
            await Task.Delay((int)(HideDelay));
            gameObject.SetActive(false);
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(this);
        }

        public GameObject GetGameObject() => gameObject;

        public string PoolId { get; set; }
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(false);
    }
}