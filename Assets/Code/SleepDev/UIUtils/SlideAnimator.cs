using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class SlideAnimator : MonoBehaviour
    {
        [SerializeField] private List<SlideElement> _elements;
        [SerializeField] private float _callbackSlideInTime;
        [SerializeField] private float _callbackSlideOutTime;
        
        public void SlideIn(Action callback)
        {
            StartCoroutine(DelayedCallback(_callbackSlideInTime, callback));
            foreach (var element in _elements)
            {
                element.GoIn();   
            }
        }

        public void SlideOut(Action callback)
        {
            StartCoroutine(DelayedCallback(_callbackSlideOutTime, callback));
            foreach (var element in _elements)
            {
                element.GoOut();   
            }
        }

        private IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
        
        
        
#if UNITY_EDITOR
        public float e_inTimAll;
        public float e_outTimAll;

        [ContextMenu("Get All")]
        public void E_GetAll()
        {
            _elements = MiscUtils.GetFromAllChildren<SlideElement>(transform);
            Dirty();
        }
        
        [ContextMenu("Set In Time All")]
        public void E_SetInTimeAll()
        {
            foreach (var el in _elements)
            {
                if (el == null)
                    continue;
                el.inTime = e_inTimAll;
                Dirty(el);
            }
            Dirty();
        }
        
        [ContextMenu("Set Out Time All")]
        public void E_SetOutTimeAll()
        {
            foreach (var el in _elements)
            {
                if (el == null)
                    continue;
                el.outTime = e_outTimAll;
                Dirty(el);
            }
            Dirty();
        }

        private void Dirty()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        private void Dirty(UnityEngine.Object obj)
        {
            UnityEditor.EditorUtility.SetDirty(obj);
        }
#endif
    }
}