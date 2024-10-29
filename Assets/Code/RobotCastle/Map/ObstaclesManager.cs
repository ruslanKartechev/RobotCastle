using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Bomber
{
    public class ObstaclesManager : MonoBehaviour
    {
        public void SetMap(IMap map)
        {
            _map = map;
        }
    
        public void SetPreset(ObstaclesPreset preset, string prefabPath)
        {
            if (_currentPreset != null)
            {
                ClearCurrentPreset();
            }
            _currentPreset = preset;
            if (preset.coordinates.Count == 0)
                return;
            var prefab = Resources.Load<GameObject>(prefabPath);
            const float startScale = .5f;
            const float upOffset = 4f;
            foreach (var coord in preset.coordinates)
            {
                _map.Grid[coord.x, coord.y].isPlayerWalkable = false;
                _map.Grid[coord.x, coord.y].isAIWalkable = false;
                var inst = Instantiate(prefab, transform).transform;
                _obstacles.Add(inst);
                var p2 = _map.GetWorldFromCell(coord);
                var p1 = p2 + new Vector3(0, upOffset, 0);
                inst.transform.position = p1;
                inst.localScale = new Vector3(startScale, startScale, startScale);
                inst.DOMove(p2, _animationTime).SetEase(Ease.OutBack);
                inst.DOScale(Vector3.one, _animationTime).SetEase(Ease.OutBack);
            }
            
        }
        
        [SerializeField] private float _animationTime = .23f;
        private IMap _map;
        private ObstaclesPreset _currentPreset;
        private List<Transform> _obstacles = new (10);
       
        private void ClearCurrentPreset()
        {
            foreach (var coord in _currentPreset.coordinates)
            {
                _map.Grid[coord.x, coord.y].isPlayerWalkable = true;
                _map.Grid[coord.x, coord.y].isAIWalkable = true;
            }

            foreach (var tr in _obstacles)
                Destroy(tr.gameObject);
            _obstacles.Clear();
        }
        
#if UNITY_EDITOR
        
        [Space(30)]
        public List<ObstaclesPreset> e_presets;
        public List<Transform> e_obstacles;

        [ContextMenu("Save")]
        private void Save()
        {
            if (e_presets == null)
                e_presets = new List<ObstaclesPreset>(3);
            var preset = new ObstaclesPreset();
            preset.coordinates = new List<Vector2Int>(e_obstacles.Count);
            foreach (var t in e_obstacles)
            {
                if (t == null || !t.gameObject.activeSelf)
                    continue;
                var pos = transform.InverseTransformPoint(t.position);
                var x = Mathf.RoundToInt(pos.x);
                var y = Mathf.RoundToInt(pos.z);
                preset.coordinates.Add(new Vector2Int(x,y));
            }
            e_presets.Add(preset);
        }
#endif
    }
}