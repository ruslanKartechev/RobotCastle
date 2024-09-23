using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class Pool
    {
        [SerializeField] private string _id;
        [SerializeField] private string _prefabPath;
        [SerializeField] private int _startCount;
        [SerializeField] private Transform _parent;
        private readonly List<IPoolItem> _objects = new (10); 
        
        public Pool(){}

        public Pool(string id, string prefabPath, int startCount)
        {
            this._id = id;
            this._prefabPath = prefabPath;
            this._startCount = startCount;
            _parent = new GameObject($"pool_{id}").transform;
        }

        public string id => _id;

        public string prefabPath => _prefabPath;

        public int startCount
        {
            get => _startCount;
            set => _startCount = value;
        }

        public void Init()
        {
            Spawn(_startCount);
        }

        public IPoolItem GetOne()
        {
            if (_objects.Count == 0)
                Spawn(_startCount);
            var val = _objects[^1];
            _objects.RemoveAt(_objects.Count - 1);
            return val;
        }

        public void Return(IPoolItem obj)
        {
            obj.PoolHide();
            _objects.Add(obj);
        }

        private void Spawn(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var prefab = Resources.Load<GameObject>(_prefabPath);
                var inst = Object.Instantiate(prefab, _parent).GetComponent<IPoolItem>();
                inst.PoolHide();
                inst.PoolId = _id;
                _objects.Add(inst);
            }
        }
        
    }
}