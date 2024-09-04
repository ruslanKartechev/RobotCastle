using System;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BaseUIScreen : MonoBehaviour, IScreenUI
    {
        [SerializeField] private Canvas _canvas;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_canvas == null)
            {
                _canvas = gameObject.GetComponent<Canvas>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        #endif

        protected void OnOpened<T>(string id, T obj) where T : IScreenUI
        {
            var ui = ServiceLocator.Get<IUIManager>();
            ui.AddAsOpened<T>(id, obj);
        }

        protected void OnClosed(string id)
        {
            var ui = ServiceLocator.Get<IUIManager>();
            ui.OnClosed(id);
        }
    }
}