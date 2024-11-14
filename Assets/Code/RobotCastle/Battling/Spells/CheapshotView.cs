using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class CheapshotView : MonoBehaviour
    {
        public void Pull(Transform sourcePoint, IHeroController enemy, float grabTime, Action grabCallback)
        {
            _sourcePoint = sourcePoint;
            _enemy = enemy;
            _grabTime = grabTime;
            _grabbedCallback = grabCallback;
            _lineRenderer.enabled = true;
            _lineRenderer.gameObject.SetActive(true);
            gameObject.SetActive(true);
            if(_working != null)
                StopCoroutine(_working);
            _working = StartCoroutine(Pulling());
        }

        public void Hide()
        {
            if(_working != null)
                StopCoroutine(_working);
            gameObject.SetActive(false);
        }

        [SerializeField] private LineRenderer _lineRenderer;
        private Transform _sourcePoint;
        private Coroutine _working;
        private IHeroController _enemy;
        private float _grabTime;
        private Action _grabbedCallback;
        private const float upCenterOffset = 1.5f;
        private const float endUpOffset = 1.2f;
        private int _pointsCount = 10;
        
        private IEnumerator Pulling()
        {
            var elapsed = 0f;

            _lineRenderer.positionCount = _pointsCount;
            while (elapsed < _grabTime)
            {
                var p1 = _sourcePoint.position;
                
                var finalEndPoint = _enemy.Components.transform.position;
                var currentEnd = Vector3.Lerp(p1, finalEndPoint, elapsed / _grabTime);
                
                var p3 = currentEnd + new Vector3(0, endUpOffset, 0);
                var p2 = Vector3.Lerp(p1, p3, .5f) + new Vector3(0, upCenterOffset, 0);
                
                for (var i = 0; i < _pointsCount; i++)
                {
                    var t = i / (_pointsCount - 1);
                    var p = Bezier.GetPosition(p1, p2, p3, t);
                    _lineRenderer.SetPosition(i, p);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            _grabbedCallback?.Invoke();
            yield return Tracking();
        }

        private IEnumerator Tracking()
        {
            while (gameObject.activeInHierarchy)
            {
                var p1 = _sourcePoint.position;
                var p3 = _enemy.Components.transform.position + new Vector3(0, endUpOffset, 0);
                var p2 = Vector3.Lerp(p1, p3, .5f) + new Vector3(0, upCenterOffset, 0);
                for (var i = 0; i < _pointsCount; i++)
                {
                    var t = i / (_pointsCount - 1);
                    var p = Bezier.GetPosition(p1, p2, p3, t);
                    _lineRenderer.SetPosition(i, p);
                }
                yield return null;
            }
        }
        
        
    }
}