using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/GameObjectFactory", fileName = "GameObjectFactory", order = 0)]
    public class GameObjectFactory : ScriptableObject
    {
        [SerializeField] private List<PrefabData> _prefabData;
        private Dictionary<string, GameObject> _idPrefabMap;
        
        #if UNITY_EDITOR
        public static GameObjectFactory GetDefault()
        {
            var s = Resources.Load<GameObjectFactory>("Config/GameObjectFactory");
            if (s == null)
            {
                CLog.LogRed($"Cannot Find GameObjectFactory in Resources/Config");
                return null;
            }
            s.Rebuild();
            return s;
        }
        #endif
        
        public void Rebuild()
        {
            _idPrefabMap = new Dictionary<string, GameObject>();
            foreach (var data in _prefabData)
            {
                _idPrefabMap.Add(data.id, data.prefab);
            }
        }

        public GameObject Spawn(string id)
        {
            return Instantiate(_idPrefabMap[id]);
        }
        
        public GameObject Spawn(GameObject prefab)
        {
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
            #endif
            return Instantiate(prefab);
        }
        
#if UNITY_EDITOR
        public GameObject SpawnAsPrefab(string id)
        {
            Rebuild();
            return UnityEditor.PrefabUtility.InstantiatePrefab(_idPrefabMap[id]) as GameObject;
        }
#endif
        
        public T Spawn<T>(string id) where T : class
        {
#if UNITY_EDITOR
            if (!CheckThrow(id))
                return null;
#endif
            return Instantiate(_idPrefabMap[id]).GetComponent<T>();
        }

        public T[] Spawn<T>(string id, int count) where T : class
        {
            #if UNITY_EDITOR
            if (!CheckThrow(id))
                return null;
            #endif
            T[] results = new T[count];
            var prefab = _idPrefabMap[id];
            for (var i = 0; i < count; i++)
            {
                results[i] = Instantiate(prefab).GetComponent<T>();
            }

            return results;
        }

        private bool CheckThrow(string id)
        {
            if (_idPrefabMap.ContainsKey(id) == false)
            {
                var msg = $"Prefab with id: {id} not present, cannot spawn";
                // throw new System.Exception(msg);
                Debug.LogError(msg);
                return false;
            }
            return true;
        }
        
        [System.Serializable]
        public class PrefabData
        {
            public string id;
            public GameObject prefab;
        }
    }
}