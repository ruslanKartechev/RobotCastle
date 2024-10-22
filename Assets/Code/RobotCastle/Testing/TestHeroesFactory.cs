using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class TestHeroesFactory : MonoBehaviour
    {
        [SerializeField] private List<HeroController> _spawned;
        [Space(10)]
        [SerializeField] private List<PackSpawnData> _spawnData;

        public List<HeroController> AllSpawned => _spawned;

        
        [ContextMenu("DeleteAll")]
        public void DeleteAll()
        {
            if (_spawned == null)
                return;
            for (var i = _spawned.Count - 1; i >= 0; i--)
            {
                if (_spawned[i] == null) continue;
                DestroyImmediate(_spawned[i].gameObject);
            }
            _spawned.Clear();
        }
        
        
        [ContextMenu("Spawn All")]
        public List<HeroController> SpawnAll()
        {
            var all = new List<HeroController>(30);
            foreach (var spawnData in _spawnData)
            {
                var res = Spawn(spawnData);
                all.AddRange(res);
            }
            _spawned = all;
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
            return all;
        }

        public List<HeroController> Spawn(PackSpawnData spawnData)
        {
            var allSpawned = new List<HeroController>(20);
            var root = spawnData.root;
            if (root == null)
                root = transform;
            var y = 0;
            var x = 0;
            var currentGridPos = Vector3.zero;
            var pack = spawnData.pack;
            foreach (var id in pack.units)
            {
                var prefabPath = $"prefabs/{pack.prefabFolder}/{id}";
                CLog.Log($"Loading from: {prefabPath}");
                var prefab = Resources.Load<GameObject>(prefabPath);
                var instance = SleepDev.MiscUtils.Spawn(prefab, root);
                currentGridPos.x = x * spawnData.spacing.x;
                currentGridPos.z = y * spawnData.spacing.y;
                instance.name = instance.name.Replace("(Clone)", "");
                x++;
                if (x >= spawnData.grid.x)
                {
                    x = 0;
                    y++;
                }
                if (y >= spawnData.grid.y)
                    y = 0;
                if (root != null)
                    instance.transform.position = root.TransformPoint(currentGridPos);
                else
                    instance.transform.position = currentGridPos;
                if (!Application.isPlaying)
                {
                    if (instance.TryGetComponent<HeroController>(out var hero))
                        allSpawned.Add(hero);
                    continue;
                }
                else
                {
                    var map = ServiceLocator.Get<Bomber.IMap>();
                    if (instance.TryGetComponent<HeroController>(out var hero))
                    {
                        hero.InitHero(id, pack.heroLevel,pack.mergeLevel, new List<ModifierProvider>());          
                        hero.Components.agent.UpdateMap(map);
                        allSpawned.Add(hero);
                    }
                }
            }
            return allSpawned;
        }
        
        
        [System.Serializable]
        public class Pack
        {
            public string prefabFolder;
            public string configFolder;
            public int heroLevel;
            public int mergeLevel;
            public List<string> units;
        }

        [System.Serializable]
        public class PackSpawnData
        {
            public Transform root;
            public Vector2Int grid = new Vector2Int(5, 2);
            public Vector2 spacing = new Vector2(1, 1);
            [Space(10)]
            public Pack pack;
        }
        
    }
}