using System.Collections;
using System.Threading.Tasks;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class OneTimeParticles : MonoBehaviour, IPoolItem
    {
        private const float HideDelay = 1.5f;
        [SerializeField] private ParticleSystem _particles;
        
        public void Show(Vector3 worldPos)
        {
            transform.position = worldPos;
            gameObject.SetActive(true);
            _particles.Play();
            StartCoroutine(DelayHide());
        }

        public GameObject GetGameObject() => gameObject;

        public string PoolId { get; set; }
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(false);
        
        private IEnumerator DelayHide()
        {
            yield return new WaitForSeconds(HideDelay);
            gameObject.SetActive(false);
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(this);
        }
    }
}