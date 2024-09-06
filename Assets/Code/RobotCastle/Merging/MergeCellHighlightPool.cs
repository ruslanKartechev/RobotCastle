using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeCellHighlightPool : MonoBehaviour, IMergeCellHighlightPool
    {
        [SerializeField] private int _startCount = 4;
        [SerializeField] private int _addCount = 4;
        [SerializeField] private string _prefabPath;
        [SerializeField] private string _prefabPath2;
        private List<CellHighlight> _poolTyp1; 
        private List<CellHighlight> _poolTyp2; 
        
        public CellHighlight GetOneType1()
        {
            if (_poolTyp1.Count == 0)
            {
                SpawnType1(_addCount);

            }
            var obj = _poolTyp1[^1];
            _poolTyp1.RemoveAt(_poolTyp1.Count-1);
            return obj;
        }

        public CellHighlight GetOneType2()
        {
            if (_poolTyp2.Count == 0)
            {
                SpawnType2(_addCount);

            }
            var obj = _poolTyp2[^1];
            _poolTyp2.RemoveAt(_poolTyp2.Count-1);
            return obj;
        }

        public void ReturnType1(CellHighlight obj) => _poolTyp1.Add(obj);
        
        public void ReturnType2(CellHighlight obj) => _poolTyp2.Add(obj);

        public int CurrentCount => _poolTyp1.Count;
        
        public void Init()
        {
            _poolTyp1 = new List<CellHighlight>(_addCount);
            _poolTyp2 = new List<CellHighlight>(_addCount);
            SpawnType1(_startCount);
            SpawnType2(_startCount);
        }

        private void SpawnType1(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var prefab = Resources.Load<CellHighlight>(_prefabPath);
                var inst = Instantiate(prefab, transform);
                inst.Hide();
                _poolTyp1.Add(inst);
            }
        }
        
        private void SpawnType2(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var prefab = Resources.Load<CellHighlight>(_prefabPath2);
                var inst = Instantiate(prefab, transform);
                inst.Hide();
                _poolTyp2.Add(inst);
            }
        }
    }
}