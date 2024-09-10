using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace Bomber
{
    public class MapBuilder : MonoBehaviour, IMapManager
    {
        [System.Serializable]
        public class PrefabData
        {
            public MapCellContent prefab;
            public Transform parent;
        }

        [SerializeField] private bool _originInCenter;
        [SerializeField] private Vector2Int _size;
        [SerializeField] private EditorMap _editorMap;
        [Space(10)] 
        [SerializeField] private bool _showEditor;
        [SerializeField] private Vector2Int _currentNodePos;
        [SerializeField] private List<PrefabData> _contentPrefabs;
        [SerializeField] private PrefabData _currentPrefab;
        private int _prefabIndex;
        private IMap _map;

        public IMap Map => _map;
        public Vector2Int Size => _size;
        public bool ShowEditor => _showEditor;
        
        public PrefabData CurrentPrefab => _currentPrefab;
        
        public Vector2Int CurrentPos
        {
            get => _currentNodePos;
            set
            {
                _currentNodePos = value;
                SetMeDirty();
            }
        }

        public EditorMapCell GetCellAt(int x, int y) => _editorMap.Rows[y].cells[x];

        public void InitRuntime()
        {
            var mm = new Map(_size, cellSize:1f, _editorMap.Rows[0].cells[0].worldPosition);
            CLog.LogWhite($"[{nameof(MapBuilder)}] Building Runtime map. WorldOrigin {mm.WorldOrigin}");
            mm.FillFromEditor(_editorMap);
            _map = mm;
        }
        
        public void BuildEmptyEditorMap()
        {
            _editorMap = new EditorMap(_size);
            _editorMap.Rows = new List<EditorMapRow>(_size.y);
            for (var rowI = 0; rowI < _size.y; rowI++)
            {
                var row = new EditorMapRow();
                _editorMap.Rows.Add(row);
                row.cells = new List<EditorMapCell>(_size.x);
                for (var x = 0; x < _size.x; x++)
                {
                    row.cells.Add(new EditorMapCell()
                    {
                        isPlayerWalkable = true,
                        isAIWalkable = true
                    });
                }
            }
#if UNITY_EDITOR
            SetMeDirty();
            #endif
        }
        
        public void NextPrefab()
        {
            _prefabIndex++;
            _prefabIndex = Mathf.Clamp(_prefabIndex, 0, _contentPrefabs.Count - 1);
            _currentPrefab = _contentPrefabs[_prefabIndex];
            SetMeDirty();
        }

        public void PrevPrefab()
        {
            _prefabIndex--;
            _prefabIndex = Mathf.Clamp(_prefabIndex, 0, _contentPrefabs.Count - 1);
            _currentPrefab = _contentPrefabs[_prefabIndex];
            SetMeDirty();
        }

        public void SpawnContentAtCurrentCell()
        {
            ClearContentAtCurrentCell();
            var cell = _editorMap.GetCell(CurrentPos);
            var data = CurrentPrefab;
#if UNITY_EDITOR
            var instance = UnityEditor.PrefabUtility.InstantiatePrefab(data.prefab, data.parent) as MapCellContent;
            cell.content = instance;
            if(instance == null)
                CLog.LogRed("Instantiated null...");
            else
            {
                instance.transform.position = cell.worldPosition;
                SetBlockTypeForCell(cell, instance);
            }
            SetMeDirty();
#endif
        }

        public void ClearContentAtCurrentCell()
        {
            var cell = _editorMap.GetCell(CurrentPos);
            if (cell.content == null)
                return;
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                DestroyImmediate(cell.content.gameObject);
                cell.content = null;
                cell.blockType = EMapBlockType.Empty;
                cell.SetWalkableIfFree();
            }
            #endif
        }

        public string CurrentPrefabName()
        {
            if (_currentPrefab == null || _currentPrefab.prefab == null)
                return "none";
            return _currentPrefab.prefab.gameObject.name;
        }

        public void ScanSetWalkable()
        {
            foreach (var row in _editorMap.Rows)
            {
                foreach (var cell in row.cells)
                {
                    cell.SetWalkableIfFree();
                }
            }
            SetMeDirty();
        }

        public void MoveCurrentCell(Vector2Int dir)
        {
            _currentNodePos += dir;
            _currentNodePos.x = Mathf.Clamp(_currentNodePos.x, 0, _size.x - 1);
            _currentNodePos.y = Mathf.Clamp(_currentNodePos.y, 0, _size.y - 1);
            SetMeDirty();
        }

        public void DrawWithRays()
        {
            const float rayDrawDuration = 5;
            for (var y = 0; y < _size.y; y++)
            {
                for (var x = 0; x < _size.x; x++)
                {
                    var pos = _editorMap.Rows[y].cells[x].worldPosition;
                    var pos2 = pos + Vector3.up * 2;
                    Debug.DrawLine(pos, pos2, Color.yellow, rayDrawDuration);
                }
            }
        }

        public void ScanToBuild()
        {
            BuildEmptyEditorMap();
            var step = 1f;
            var castRad = .2f;
            var upPos = 10f;
            var castMaxDist = 11f;
            Vector3 origin;
            if (_originInCenter)
                origin = GetOriginPos();
            else
                origin = transform.position;
            
            const float rayDrawDuration = 5;
            for (var y = 0; y < _size.y; y++)
            {
                for (var x = 0; x < _size.x; x++)
                {
                    var cell = _editorMap.Rows[y].cells[x];
                    var pos = origin + new Vector3(x * step, upPos, y * step);
                    _editorMap.Rows[y].cells[x].worldPosition = new Vector3(pos.x, 0f, pos.z);
                    var hits = Physics.RaycastAll(pos, Vector3.down, castMaxDist);
                    Debug.DrawLine(pos, pos - Vector3.up * castMaxDist, Color.yellow, rayDrawDuration);
                    foreach (var hit in hits)
                    {
                        if (hit.collider.gameObject.TryGetComponent<MapCellContent>(out var res))
                        {
                            cell.content = res;
                            SetBlockTypeForCell(cell, res);
                            break;
                        }
                        else
                        {
                            cell.content = null;
                            SetBlockTypeForCell(cell, res);
                        }
                    }
                    cell.SetWalkableIfFree();
                }
            }
            SetMeDirty();
        }

        public void TryInitTreasure()
        {
#if UNITY_EDITOR
            // var treasureManager = FindObjectOfType<LootTreasureManager>();
            // if (treasureManager == null)
            // {
            //     CLog.LogRed($"treasureManager == null");
            //     return;
            // }
            // treasureManager.E_GetAllTreasures();
#endif
        }
        
        private Vector3 GetOriginPos()
        {
            var step = 1f;
            var origin = transform.position;
            if (_size.x % 2 != 0)
                origin.x -= (int)(_size.x * step / 2f);
            else
                origin.x -= (_size.x * step / 2f) - .5f;
            
            if (_size.y % 2 != 0)
                origin.z -= (int)(_size.y * step / 2f);
            else
                origin.z -= (_size.y * step / 2f) - .5f;
            return origin;
        }

        private void SetBlockTypeForCell(EditorMapCell cell, MapCellContent content)
        {
            if (content == null)
            {
                cell.blockType = EMapBlockType.Empty;
                return;
            }
            switch (content)
            {
                case MapCellBreakableWall:
                    cell.blockType = EMapBlockType.SoftWall;
                    break;
                case MapCellHardWall:
                    cell.blockType = EMapBlockType.HardWall;
                    break;
                default:
                    cell.blockType = EMapBlockType.Other;
                    break;
            }
        }
        
        private void SetMeDirty()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}