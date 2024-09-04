using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SleepDev.Chunks
{
    [System.Serializable]
    public class ChunksBuilder
    {
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private float _chunkSize;
        [SerializeField] private List<BaseChunk> _chunks;
        [SerializeField] private List<BaseChunk> _prefabs;
        private int _prefabIndex; 
        public Transform Parent { get; set; }
        public List<BaseChunk> Chunks => _chunks;


#if UNITY_EDITOR

        public void DeleteSpawned()
        {
            foreach (var ch in _chunks)
            {
                if(ch != null)
                    Object.DestroyImmediate(ch.gameObject);
            }
            _chunks.Clear();
        }
        
        public void BuildChunks()
        {
            var centerPoint = Parent.position;
            var startPoint = centerPoint;
            float xOffset = (_gridSize.x / 2);
            if(_gridSize.x % 2 == 0)
                xOffset -= .5f;
            float yOffset = (_gridSize.y / 2);
            if(_gridSize.y % 2 == 0)
                yOffset -= .5f;

            startPoint.x -= xOffset * _chunkSize;
            startPoint.z -= yOffset * _chunkSize;
            _prefabIndex = 0;
            for (var x = 0; x < _gridSize.x; x++)
            {
                for (var y = 0; y < _gridSize.x; y++)
                {
                    if (_prefabIndex > _prefabs.Count - 1)
                        _prefabIndex = 0;
                    var instance = ((GameObject)PrefabUtility.InstantiatePrefab(_prefabs[_prefabIndex], Parent)).GetComponent<BaseChunk>();
                    _chunks.Add(instance);
                    var pos = startPoint + new Vector3(x,0,y) * _chunkSize;
                    instance.transform.position = pos;
                    _prefabIndex++;
                }                
            }
        }
        
#endif
    }
}