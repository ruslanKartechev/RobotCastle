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

        public void PlayHitParticles()
        {
            var p = _hitParticles[0];
            p.gameObject.SetActive(true);
            p.Play();
        }

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
            Tracking(target, _token.Token);
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
        
        private async void Tracking(Transform target, CancellationToken token)
        {
            while (!token.IsCancellationRequested && gameObject.activeSelf)
            {
                transform.position = target.position;
                await Task.Yield();
                if(token.IsCancellationRequested)
                    return;
            }
        }
    }
}