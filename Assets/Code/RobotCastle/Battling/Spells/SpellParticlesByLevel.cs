using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SleepDev;
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

        public void PlayAtPoint(Transform point, int level)
        {
            gameObject.SetActive(true);
            transform.CopyPosRot(point);
            _particles[level].gameObject.SetActive(true);
            _particles[level].Play();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show(int level)
        {
            StartCoroutine(DelayedHide(_duration));
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

        }

        private IEnumerator DelayedHide(float time)
        {
            gameObject.SetActive(true);
            yield return new WaitForSeconds(time);
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