﻿using System;
using System.Collections.Generic;
using System.IO;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Saving
{
    public class DataSaver : MonoBehaviour, IDataSaver
    {
        public static DataSaver Create()
        {
            var go = new GameObject("data_saver");
            DontDestroyOnLoad(go);
            var me = go.AddComponent<DataSaver>();
            me.Init();
            return me;
        }

        private string _persistentPath;
        private readonly Dictionary<Type, object> _loadedData = new(10);
        private bool _didLoad;

        public bool DidLoad => _didLoad;

        public void Init()
        {
            _persistentPath = Application.persistentDataPath;
        }
	    
        public void SaveAll()
        {
            foreach (var (type, obj) in _loadedData)
            {
                var typeName = type.ToString() + ".json";
                var path = Path.Join(_persistentPath, typeName);
                var content = JsonUtility.ToJson(obj);
                File.WriteAllText(path, content);
            }
        }

        public T LoadSave<T>(T defaultObject)
        {
            return Load<T>(defaultObject);
        }

        protected T Load<T>(T defaultObject = default) 
        {
            var path = Path.Join(_persistentPath, $"{typeof(T).ToString()}.json");
            if (File.Exists(path) == false)
            {
                CLog.Log($"[IDataSaver] Path Does not exist: {typeof(T).ToString()}");
                _loadedData.Add(typeof(T), defaultObject);
                return defaultObject;
            }
            var str = File.ReadAllText(path);
            if (str.Length > 0)
            {
                var loadedData = JsonUtility.FromJson<T>(str);
                if (loadedData != null)
                {
                    CLog.Log($"[IDataSaver] Successfully Loaded: {typeof(T).ToString()}");
                    _loadedData.Add(typeof(T), loadedData);
                    return loadedData;
                }
            }
            _loadedData.Add(typeof(T), defaultObject);
            CLog.Log($"[IDataSaver] FAILED to load: {typeof(T).ToString()}");
            return defaultObject;
        }
	    
        protected void Save<T>(T data)
        {
            var path = Path.Join(_persistentPath, $"{typeof(T).ToString()}.json");
            var str = JsonUtility.ToJson(data);
            File.WriteAllText(path, str);
        }

        public T GetData<T>()
        {
            return (T)_loadedData[typeof(T)];
        }

        public  void Delete<T>()
        {
            var path = Path.Join(_persistentPath, $"{typeof(T).ToString()}.json");
            File.Delete(path);
        }
        
        public static void DeleteFile<T>()
        {
            var path = Path.Join(Application.persistentDataPath, $"{typeof(T).ToString()}.json");
            File.Delete(path);
        }
    }
}