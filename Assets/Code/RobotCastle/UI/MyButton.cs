using System;
using System.Collections.Generic;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class MyButton : MonoBehaviour, IButtonInput
    {
        [SerializeField] protected Button _btn;
        [SerializeField] protected bool _doRespond;
        [SerializeField] private SoundID _sound;
        protected readonly List<Action> _callbacks = new List<Action>(5);

        private void Start()
        {
            if (_sound == null)
                _sound = Resources.Load<SoundID>("sounds/s_click");
        }

        protected virtual void OnEnable()
        {
            _btn.onClick.AddListener(MainCallback);
            _btn.onClick.AddListener(FXCallback);
        }

        protected virtual void OnDisable()
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

        public void RemoveMainCallback(Action callback)
        {
            _callbacks.Remove(callback);
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

        public bool IsInteractable() => _btn.interactable;

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
            for (var i = _callbacks.Count-1; i >= 0; i--)
            {
                var callback = _callbacks[i];
                callback.Invoke();
            }
        }

        private void FXCallback()
        {
            if (!_doRespond) return;
            SoundManager.Inst.Play(_sound, false);
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