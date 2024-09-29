using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellParticleOnGridEffect : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _particles;
        [SerializeField] private float _duration = 1.5f;
        
        public async void Show(List<Vector3> positions)
        {
            var count = positions.Count;
            for (var i = 0; i < count && i < _particles.Count; i++)
            {
                _particles[i].SetActive(true);
                _particles[i].transform.position = positions[i];
            }
            for (var i = count; i < _particles.Count; i++)
                _particles[i].SetActive(false);
            gameObject.SetActive(true);
            await Task.Delay((int)(1000 * _duration));
            if (Application.isPlaying == false) return;
            gameObject.SetActive(false);
        }
    }
}