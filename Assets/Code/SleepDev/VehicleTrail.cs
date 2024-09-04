using UnityEngine;

namespace SleepDev
{
    public class VehicleTrail : MonoBehaviour
    {
        [SerializeField] private Transform[] _points;
        private ParticleSystem[] _particles;
        
        
        public void Spawn()
        {
            _particles = new ParticleSystem[_points.Length];
            var prefab = EnvironmentState.VehicleTrailPrefab();
            for (var i = 0; i < _points.Length; i++)
            {
                var inst = Instantiate(prefab, _points[i].position, _points[i].rotation, _points[i]);
                inst.transform.localScale = Vector3.one;
                _particles[i] = inst;
            }
        }
        
        public void Off()
        {
            if (_particles == null)
                return;
            foreach (var pp in _particles)
                pp.gameObject.SetActive(false);
        }

        public void On()
        {
            foreach (var pp in _particles)
                pp.gameObject.SetActive(true);
        }
    }
}