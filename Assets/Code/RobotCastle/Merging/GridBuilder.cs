using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class GridBuilder : MonoBehaviour
    {
        [SerializeField] private bool _originCenter;
        [SerializeField] private bool _deleteBeforeNew;
        [SerializeField] private List<GameObject> _cellViewGameObjects;
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private Vector2 _cellSize; 
        [SerializeField] private Transform _parent;
        [SerializeField] private GameObject _prefab;
 
        #if UNITY_EDITOR

        public void DeleteAll()
        {
            if (_cellViewGameObjects == null)
                return;
            for (var i = 0; i < _cellViewGameObjects.Count; i++)
                SleepDev.MiscUtils.Destroy(_cellViewGameObjects[i]);
            _cellViewGameObjects.Clear();
        }
        
        [ContextMenu("BuildGrid")]
        public void BuildGrid()
        {
            if(_deleteBeforeNew)
                DeleteAll();
            var center = transform.position;
            if (_originCenter)
            {
                center = Vector3.zero;
                center.x -= (_gridSize.x * .5f * _cellSize.x);
                if (_gridSize.x % 2 > 0)
                    center.x += _cellSize.x * .5f;
            
                center.z -= (_gridSize.y * .5f * _cellSize.y);
                if (_gridSize.y % 2 > 0)
                    center.z += _cellSize.y * .5f;
            }
            
            _cellViewGameObjects = new List<GameObject>(_gridSize.x * _gridSize.y);
            for (var y = 0; y < _gridSize.y; y++)
            {
                for (var x = 0; x < _gridSize.x; x++)
                {
                    var pos = center + new Vector3(x * _cellSize.x, 0f, y * _cellSize.y);
                    var cellInstance = SleepDev.MiscUtils.Spawn(_prefab, _parent);
                    cellInstance.gameObject.name = $"cell_{x}_{y}";
                    cellInstance.transform.position = pos;
                    _cellViewGameObjects.Add(cellInstance);
                    UnityEditor.EditorUtility.SetDirty(cellInstance);
                }
            }
        }
        #endif
        
    }
}