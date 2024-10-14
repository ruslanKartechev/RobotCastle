using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class MyButton : MonoBehaviour, IButtonInput
    {
        [SerializeField] private Button _btn;
        [SerializeField] private bool _doRespond;
        private readonly List<Action> _callbacks = new List<Action>(5);

        private  void OnEnable()
        {
            _btn.onClick.AddListener(MainCallback);
            _btn.onClick.AddListener(FXCallback);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveListener(MainCallback);
            _btn.onClick.RemoveListener(FXCallback);
        }

        /// <summary>
        /// Will add a new callback, if not already added
        /// </summary>
        /// <param name="callback"></param>
        public void AddMainCallback(Action callback)
        {
            if (_callbacks.Contains(callback))
                return;
            _callbacks.Add(callback);
        }


        /// <summary>
        /// Will delete all existing non-fx callbacks and set this one
        /// </summary>
        /// <param name="callback"></param>
        public void OverrideMainCallback(Action callback)
        {
            _callbacks.Clear();
            _callbacks.Add(callback);
        }

        public void SetInteractable(bool interactable, bool visually = true)
        {
            if (interactable)
            {
                _btn.interactable = true;
                _doRespond = true;
            }
            else
            {
                _doRespond = false;
                if (visually)
                    _btn.interactable = false;
            }
        }

        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        private void MainCallback()
        {
            if (!_doRespond) return;
            foreach (var callback in _callbacks)
                callback.Invoke();
        }

        private void FXCallback()
        {
            if (!_doRespond) return;
            // play sound, etc
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_btn == null)
            {
                _btn = gameObject.GetComponent<Button>();
                if(_btn != null)
                    UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}