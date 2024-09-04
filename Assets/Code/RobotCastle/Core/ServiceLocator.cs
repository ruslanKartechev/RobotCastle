using System;
using System.Collections.Generic;

namespace RobotCastle.Core
{
    public class ServiceLocator
    {
        private static ServiceLocator _inst;
        
        public static void Create()
        {
            if (_inst != null)
                return;
            _inst = new ServiceLocator();
        }
        
        public static void Bind<T>(object obj) => _inst.MBind<T>(obj);
        public static void Unbind<T>() => _inst.MUnbind<T>();
        public static bool GetIfContains<T>(out T obj) => _inst.MGetIfContains<T>(out obj);
        public static T Get<T>() => _inst.MGet<T>();

        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>(100);

        public void MBind<T>(object obj)
        {
            var type = typeof(T);
            if (_instances.ContainsKey(type))
                _instances[type] = obj;
            else
                _instances.Add(type, obj);
        }

        public void MUnbind<T>()
        {
            _instances.Remove(typeof(T));
        }

        public T MGet<T>()
        {
            var type = typeof(T);
            if (_instances.ContainsKey(type))
                return (T)_inst._instances[type];
            return default;
        }

        
        public bool MGetIfContains<T>(out T obj)
        {
            var type = typeof(T);
            if (_instances.ContainsKey(type))
            {
                obj = (T)_instances[type];
                return true;
            }
            obj = default;
            return false;
        }
    }
}