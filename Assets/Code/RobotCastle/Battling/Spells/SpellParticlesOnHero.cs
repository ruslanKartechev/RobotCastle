using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellParticlesOnHero : MonoBehaviour
    {
        private const float duration = 2f;
        
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private List<ParticleSystem> _hitParticles;
        private CancellationTokenSource _token;
        private Coroutine _tracking;

        public void PlayHitParticles(int lvl)
        {
            var p = _hitParticles[lvl];
            p.gameObject.SetActive(true);
            p.Play();
        }

        public void ShowUntilOff(Transform target)
        {
            _token = new CancellationTokenSource();
            gameObject.SetActive(true);
            _particles.gameObject.SetActive(true);
            _particles.Play();
            if(_tracking != null)
                StopCoroutine(_tracking);
            _tracking = StartCoroutine(Tracking(target));
        }
        
        public void ShowTrackingDefaultDuration(Transform target)
        {
            _token = new CancellationTokenSource();
            gameObject.SetActive(true);
            _particles.gameObject.SetActive(true);
            _particles.Play();
            TrackingForTime(target, duration, _token.Token);
        }

        private void OnDisable()
        {
            if (_particles != null)
                _particles.transform.parent = transform;
            _token?.Cancel();
        }

        private async void TrackingForTime(Transform target, float time, CancellationToken token)
        {
            while (time >= 0)
            {
                transform.position = target.position;
                time -= Time.deltaTime;
                await Task.Yield();
                if(token.IsCancellationRequested)
                    return;
            }
            gameObject.SetActive(false);
        }
        
        private IEnumerator Tracking(Transform target)
        {
            while (gameObject.activeSelf)
            {
                transform.position = target.position;
                yield return null;
            }
        }
    }
}