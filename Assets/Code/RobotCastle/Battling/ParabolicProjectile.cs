using System;
using System.Collections;
using System.Collections.Generic;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class ParabolicProjectile : MonoBehaviour, IProjectile, IPoolItem
    {
     
        public void LaunchProjectile(Transform startPoint, Transform endPoint, float speed, Action<object> hitCallback, object target)
        {
            _hitCallback = hitCallback;
            _target = target;
            var dirVec = endPoint.position - startPoint.position;
            dirVec.y = 0f;
            var rot = Quaternion.LookRotation(dirVec);
            transform.SetPositionAndRotation(startPoint.position, rot);
            gameObject.SetActive(true);

            foreach (var go in _disableOnHitGo)
                go.SetActive(true);
            
            StartCoroutine(Moving(startPoint, endPoint, speed));
        }

        public GameObject GetGameObject() => gameObject;
        
        public string PoolId { get; set; }
        
        public void PoolHide() => gameObject.SetActive(false);
        
        public void PoolShow() => gameObject.SetActive(true);

        [SerializeField] private float _height = 10;
        [SerializeField] private bool _isPooled = true;
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private List<GameObject> _disableOnHitGo;
        private Action<object> _hitCallback;
        private object _target;
        
        private IEnumerator Moving(Transform startPoint, Transform endPoint, float speed)
        {
            var elapsed = 0f;
            var startPos = startPoint.position;
            var endPos = endPoint.position;
            endPos.y = startPoint.position.y;
            
            var distance = (endPos - startPos).magnitude / speed;
            var time = distance / speed;
            
            while (elapsed <= time)
            {
                var p1 = startPos;
                var p3 = endPoint.position;
                var t= elapsed / time;
                var p2 = Vector3.Lerp(p1, p3, .5f) + Vector3.up * _height;
                transform.position = Bezier.GetPosition(p1, p2, p3, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = endPoint.position;
            foreach (var go in _disableOnHitGo)
                go.SetActive(false);

            _hitCallback?.Invoke(_target);
            if (_hitParticles != null)
            {
                _hitParticles.gameObject.SetActive(true);
                _hitParticles.Play();
                yield return new WaitForSeconds(2f);
            }
            if(_isPooled)
                ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(this);
            else
                gameObject.SetActive(false);
        }
    }
}