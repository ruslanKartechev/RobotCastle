using System.Threading.Tasks;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class OneTimeParticles : MonoBehaviour, IPoolItem
    {
        private const float HideDelay = 1.5f * 1000;
        [SerializeField] private ParticleSystem _particles;
        
        public async void Show(Vector3 worldPos)
        {
            transform.position = worldPos;
            gameObject.SetActive(true);
            _particles.Play();
            await Task.Delay((int)(HideDelay));
            if (!Application.isPlaying || gameObject == null) return;
            gameObject.SetActive(false);
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(this);
        }

        public GameObject GetGameObject() => gameObject;

        public string PoolId { get; set; }
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(false);
    }
}