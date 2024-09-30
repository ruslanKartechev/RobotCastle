using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellParticleOnGridEffect : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _particles;
        [SerializeField] private float _duration = 1.5f;
        
        public async void Show(List<Vector3> positions)
        {
            gameObject.SetActive(true);
            var count = positions.Count;
            for (var i = 0; i < count && i < _particles.Count; i++)
            {
                _particles[i].gameObject.SetActive(true);
                _particles[i].transform.position = positions[i];
                _particles[i].Play();
            }
            for (var i = count; i < _particles.Count; i++)
                _particles[i].gameObject.SetActive(false);
            
            await Task.Delay((int)(1000 * _duration));
            if (Application.isPlaying == false) return;
            gameObject.SetActive(false);
        }
    }
}