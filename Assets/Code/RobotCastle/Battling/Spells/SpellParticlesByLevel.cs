using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellParticlesByLevel : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _particles;
        [SerializeField] private float _duration = 1.5f;

        public void ShowUnTillOff(int level, Transform point)
        {
            foreach (var tp in _particles)
                tp.gameObject.SetActive(false);
            gameObject.SetActive(true);
            var p = _particles[level];
            p.gameObject.SetActive(true);
            p.Play();
            StartCoroutine(TrackingPosition(point));
        }

        public void PlayLevelAtPoint(Vector3 point, int level)
        {
            gameObject.SetActive(true);
            _particles[level].transform.position = point;
            _particles[level].gameObject.SetActive(true);
            _particles[level].Play();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public async void Show(int level)
        {
            gameObject.SetActive(true);
            for (var i = 0; i < _particles.Count; i++)
            {
                var p = _particles[i];
                p.gameObject.SetActive(i == level);
                if (i == level)
                {
                    p.gameObject.SetActive(true);
                    p.Play();
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }
            await Task.Delay((int)(1000 * _duration));
            if (Application.isPlaying == false)
                return;
            gameObject.SetActive(false);
        }
        
        private IEnumerator TrackingPosition(Transform point)
        {
            while (true)
            {
                transform.SetPositionAndRotation(point.position, point.rotation);
                yield return null;
            }
        }
    }
}