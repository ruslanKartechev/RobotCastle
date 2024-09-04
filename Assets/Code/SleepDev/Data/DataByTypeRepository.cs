using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public class DataByTypeRepository<TData, TType>
    {
        [SerializeField] private List<DataByType<TData, TType>> _data;

        public IList<DataByType<TData, TType>> Data => _data;
        private Dictionary<TType, TData> _table;

        public virtual void Init()
        {
            _table = new Dictionary<TType, TData>(_data.Count);
            foreach (var dt in _data)
            {
                _table.Add(dt.type, dt.data);
            }
        }

        public virtual TData GetData(TType type)
        {
            return _table[type];
        }
    }
}