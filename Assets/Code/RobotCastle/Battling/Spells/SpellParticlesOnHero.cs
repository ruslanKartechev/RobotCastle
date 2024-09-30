using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellParticlesOnHero : MonoBehaviour
    {
        private const float duration = 1.5f;
        
        [SerializeField] private ParticleSystem _particles;
        private CancellationTokenSource _token;
        
        public void Show(Transform target)
        {
            _token = new CancellationTokenSource();
            gameObject.SetActive(true);
            Tracking(target, _token.Token);
            _particles.Play();
        }

        private void OnDisable()
        {
            _token?.Cancel();
        }

        private async void Tracking(Transform target, CancellationToken token)
        {
            var time = duration;
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
        
        
    }
}