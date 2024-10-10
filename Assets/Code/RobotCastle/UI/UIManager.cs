using System;
using System.Collections.Generic;
using RobotCastle.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobotCastle.UI
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        public static UIManager Create()
        {
            var go = new GameObject("ui_manager");
            var ui = go.AddComponent<UIManager>();
            DontDestroyOnLoad(go);
            return ui;
        }

        private Dictionary<string, IScreenUI> _openedScreens = new(20);
        private Dictionary<string, Action> _closedCallbacks = new(20);

        public Canvas ParentCanvas { get; set; }

        public void Refresh()
        {
            _openedScreens.Clear();
            _closedCallbacks.Clear();
        }

        public T Show<T>(string id, Action onClosed) where T : IScreenUI
        {
            T obj;
            if (_openedScreens.ContainsKey(id))
            {
                obj = (T)_openedScreens[id];
                if (obj != null)
                {
                    _closedCallbacks[id] = onClosed;
                    return obj;       
                }
                else
                    Debug.LogError($"OBJECT IS NULL {id}");
            }

            var path = $"prefabs/ui/{id}";
            var prefab = Resources.Load<GameObject>(path);
            obj = UnityEngine.Object.Instantiate(prefab, ParentCanvas.transform).GetComponent<T>();
            _openedScreens.Add(id, obj);
            _closedCallbacks.Add(id, onClosed);
            return obj;
        }

        public T GetIfShown<T>(string id) where T : IScreenUI
        {
            if(_openedScreens.ContainsKey(id))
                return (T)_openedScreens[id];
            return default;
        }

        public void ShowScene(string id, Action onClosed)
        {
           var scene = NamingData.Inst.uiData[id];
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            _closedCallbacks.Add(id, onClosed);
        }

        public void AddAsShown<T>(string id, T obj) where T : IScreenUI
        {
            if (_openedScreens.ContainsKey(id))
                _openedScreens[id] = obj;
            else
                _openedScreens.Add(id, obj);
        }

        public void OnClosed(string id)
        {
            _openedScreens.Remove(id);
            var callback = _closedCallbacks[id];
            _closedCallbacks.Remove(id);
            callback?.Invoke();
        }
    }
}