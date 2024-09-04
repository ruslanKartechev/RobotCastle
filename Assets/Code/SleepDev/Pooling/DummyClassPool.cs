using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class DummyClass : MonoBehaviour, IPooledObject<DummyClass>
    {
        public IObjectPool<DummyClass> Pool { get; set; }
        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Parent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public DummyClass Obj => this;
        
    }
    
    [System.Serializable]
    public class DummyClassPool : IObjectPool<DummyClass>
    {
        private const int ExtensionSize = 10;
        
        [SerializeField] private string _id;
        private List<IPooledObject<DummyClass>> _instances = new List<IPooledObject<DummyClass>>();
        private GameObjectFactory _factory;

        public GameObjectFactory factory
        {
            get => _factory;
            set => _factory = value;
        }

        public string ID
        {
            get => _id;
            set => _id = value;
        }
        public int CurrentSize => _instances.Count;

        public void BuildPool(int size)
        {
            var objs = factory.Spawn<IPooledObject<DummyClass>>(ID, size);
            foreach (var ob in objs)
            {
                ob.Pool = this;
                _instances.Add(ob);
            }
        }

        public DummyClass GetObject()
        {
            if (_instances.Count == 0)
            {
                BuildPool(ExtensionSize);
            }
            var item = _instances[^1];
            _instances.RemoveAt(_instances.Count-1);
            return item.Obj;
        }

        public void ReturnObject(IPooledObject<DummyClass> obj)
        {
            _instances.Add(obj);
        }

        public void ClearPool()
        {
            foreach (var instance in _instances)
            {
                instance.Destroy();
            }
        }

        public DummyClass[] GetObjects(int count)
        {
            var arr = new DummyClass[count];
            for (var i = 0; i < count; i++)
            {
                arr[i] = GetObject();
            }
            return arr;
        }

    }
}