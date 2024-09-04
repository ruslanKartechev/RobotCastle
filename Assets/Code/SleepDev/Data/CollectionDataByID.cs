using System;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public class CollectionDataByID<T>
    {
        [SerializeField] protected List<DataByID<T>> _data;

        [NonSerialized] protected Dictionary<string, T> _table;
        [NonSerialized] private bool _tableBuilt = false;

        public bool TableBuilt => _tableBuilt;
        
        public void InitTable()
        {
            if (_tableBuilt)
                return;
            _table = new Dictionary<string, T>(_data.Count);
            foreach (var vv in _data)
                _table.Add(vv.id, vv.obj);
            _tableBuilt = true;
        }

        public void AddItem(DataByID<T> data)
        {
            _data.Add(data);
        }

        public T GetObject(string id)
        {
            return _table[id];
        }

        public void Clear()
        {
            _data.Clear();
            _table?.Clear();
        }
    }
}