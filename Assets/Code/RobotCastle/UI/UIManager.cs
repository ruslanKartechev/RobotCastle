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
            if (_openedScreens.ContainsKey(id))
            {
                var obj = _openedScreens[id];
                _closedCallbacks[id] = onClosed;
                return (T)obj;
            }
            else
            {
                var path = $"prefabs/ui/{id}";
                var prefab = Resources.Load<GameObject>(path);
                var obj = UnityEngine.Object.Instantiate(prefab, ParentCanvas.transform).GetComponent<T>();
                _openedScreens.Add(id, obj);
                _closedCallbacks.Add(id, onClosed);
                return obj;
            }
        }

        public void ShowScene(string id, Action onClosed)
        {
           var scene = NamingData.Inst.uiData[id];
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            _closedCallbacks.Add(id, onClosed);
        }

        public void AddAsOpened<T>(string id, T obj) where T : IScreenUI
        {
            _openedScreens.Add(id, obj);
        }

        public void OnClosed(string id)
        {
            _openedScreens.Remove(id);
            _closedCallbacks.Remove(id);
        }
    }
}