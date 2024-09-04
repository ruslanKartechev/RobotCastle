using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class FadeAnimator : MonoBehaviour
    {
        [SerializeField] private List<FadeElement> _elements;
        [SerializeField] private float _callbackSlideInTime;
        [SerializeField] private float _callbackSlideOutTime;
        
        public void FadeIn(Action callback)
        {
            StartCoroutine(DelayedCallback(_callbackSlideInTime, callback));
            foreach (var element in _elements)
                element.FadeIn();   
        }

        public void FadeOut(Action callback)
        {
            StartCoroutine(DelayedCallback(_callbackSlideOutTime, callback));
            foreach (var element in _elements)
                element.FadeOut();   
        }

        private IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }


        #region Editor
#if UNITY_EDITOR
        public float durationInAll;
        public float durationOutAll;
#if HAS_DOTWEEN
        public DG.Tweening.Ease easeAll;
#endif
        [ContextMenu("Get All")]
        public void E_GetAll()
        {
            _elements = MiscUtils.GetFromAllChildren<FadeElement>(transform);
            Dirty();
        }
        
        [ContextMenu("Set Ease All")]
        public void E_SetEaseAll()
        {
            foreach (var el in _elements)
            {
                if (el == null)
                    continue;
#if HAS_DOTWEEN
                el.ease = easeAll;
 #endif              
                Dirty(el);
            }
            Dirty();
        }
        
        [ContextMenu("Set In Time All")]
        public void E_SetInTimeAll()
        {
            foreach (var el in _elements)
            {
                if (el == null)
                    continue;
                el.durationIn = durationInAll;
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
                el.durationOut = durationOutAll;
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

        [ContextMenu("EditorFadeIn")]
        public void E_FadeIn()
        {
            FadeIn(() =>
            {
                //Debug.Log("Fade in end");
            });
        }

        [ContextMenu("EditorFadeOut")]
        public void E_FadeOut()
        {
            FadeOut(() =>
            {
                //Debug.Log("Fade out end");
            });
        }

#endif
    }
    #endregion
}
