using System;
using System.Collections;
using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SimpleProjectile : MonoBehaviour, IProjectile, IPoolItem
    {
        [SerializeField] private ParticleSystem _hitParticles;
        private Action<object> _hitCallback;
        private object _target;
        
        public void LaunchProjectile(Transform startPoint, Transform endPoint, float speed, Action<object> hitCallback, object target)
        {
            _hitCallback = hitCallback;
            _target = target;
            transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
            gameObject.SetActive(true);
            StartCoroutine(Moving(startPoint, endPoint, speed));
        }

        public GameObject GetGameObject() => gameObject;
        public string PoolId { get; set; }
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(true);
        
        private IEnumerator Moving(Transform startPoint, Transform endPoint, float speed)
        {
            var elapsed = 0f;
            var endPos = endPoint.position;
            endPos.y = startPoint.position.y;
            
            var distance = (endPos - startPoint.position).magnitude / speed;
            var time = distance / speed;
            while (elapsed <= time)
            {
                endPos = endPoint.position;
                endPos.y = startPoint.position.y;
                transform.position = Vector3.Lerp(startPoint.position, endPos, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
            if (_hitParticles != null)
            {
                _hitParticles.transform.parent = transform.parent;
                _hitParticles.transform.position = transform.position;
                _hitParticles.gameObject.SetActive(true);
                _hitParticles.Play();
            }
            _hitCallback?.Invoke(_target);
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(this);
        }
    }
}