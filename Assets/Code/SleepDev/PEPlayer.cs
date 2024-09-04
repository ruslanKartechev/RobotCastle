using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/ExplosionPlayer", fileName = "ExplosionPlayer", order = 0)]
    public class PEPlayer : ScriptableObject
    {
        [SerializeField] private List<ParticleSystem> _particles;
        
        private Dictionary<string, ParticleSystem> _map;

        // [System.Serializable]
        // public class Data
        // {
        //     public string id;
        //     public ParticleSystem prefab;
        // }

        private void OnEnable()
        {
            _map = new Dictionary<string, ParticleSystem>(_particles.Count);
            foreach (var pp in _particles)
                _map.Add(pp.gameObject.name, pp);
#if !UNITY_EDITOR
            _particles.Clear();
#endif
        }

        public void Play(string id, Transform point)
        {
            Play(id, point.position, point.rotation, point.lossyScale.x);
        }

        public void Play(string id, Vector3 position)
        {
            Play(id, position, Quaternion.identity, 1f);
        }
        
        public void Play(string id, Vector3 position, float scale)
        {
            Play(id, position, Quaternion.identity, scale);
        }
        
        public void Play(string id, Vector3 position, Quaternion rotation, float scale)
        {
            var inst = Instantiate(_map[id]);
            inst.transform.SetPositionAndRotation(position, rotation);
            inst.transform.localScale = Vector3.one * scale;
            inst.Play();
        }

        public void PlayParented(string id, Transform parent)
        {
            var inst = Instantiate(_map[id],parent);
            inst.transform.SetPositionAndRotation(parent.position, parent.rotation);
            inst.transform.localScale = Vector3.one;
            inst.Play();
        }
    }
}