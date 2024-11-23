using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
namespace SleepDev
{
    public class PopAnimator : MonoBehaviour
    {
        [SerializeField] private List<PopElement> _elements;

        private bool _isDone;
        public bool IsDone => _isDone;

        public void ZeroScale()
        {
            foreach (var el in _elements)
            {
                el.transform.localScale = Vector3.zero;
            }
        }

        public void ZeroAndPlay(Action onDone = null)
        {
            gameObject.SetActive(true);
            ZeroScale();
            StartCoroutine(ScalingUp(onDone));
        }

        public void PlayBackwards(Action onDone = null)
        {
            StartCoroutine(ScalingDown(onDone));
        }
        
        public IEnumerator ScalingDown(Action onDone = null)
        {
            _isDone = false;
            var totalTime = 0f;
            var timeMult = .5f;
            foreach (var pop in _elements)
                totalTime += pop.Delay * timeMult;
            var lastDur =_elements[^1].Duration * timeMult;
            foreach (var pop in _elements)
            {
                yield return new WaitForSeconds(pop.Delay * timeMult);
                pop.ScaleDown();
            }
            yield return new WaitForSeconds(lastDur);
            _isDone = true;
            onDone?.Invoke();
        }
        
        public IEnumerator ScalingUp(Action onDone = null)
        {
            _isDone = false;
            var totalTime = 0f;
            foreach (var pop in _elements)
                totalTime += pop.Delay;
            var lastDur =_elements[^1].Duration;
            
            foreach (var pop in _elements)
            {
                if(pop.Delay > 0)
                    yield return new WaitForSeconds(pop.Delay);
                pop.ScaleUp();
            }
            yield return new WaitForSeconds(lastDur);
            _isDone = true;
            onDone?.Invoke();
        }

        #if UNITY_EDITOR
        [Space(20)] 
        public float e_durationAll;
        public float e_delayAll;
#if HAS_DOTWEEN
        public Ease e_easeAll;
#endif        
        public List<GameObject> e_buildFrom;

        [ContextMenu("Play")]
        public void E_Play()
        {
            ZeroAndPlay();
        }

        public void E_Build()
        {
            _elements.Clear();
            foreach (var go in e_buildFrom)
            {
                var element = go.GetComponent<PopElement>();
                if (element == null)
                    element = go.AddComponent<SimplePopElement>();
                element.Delay = e_delayAll;
                element.Duration = e_durationAll;
                var pp = (SimplePopElement)element;
#if HAS_DOTWEEN
                if(pp != null)
                    pp.Ease = e_easeAll;
 #endif              
                EditorUtility.SetDirty(element);
                _elements.Add(element);
            }
            EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("Hide")]
        public void E_Hide()
        {
            ZeroAndPlay();
        }

        [ContextMenu("Set Delay All")]
        public void SetDelayAll()
        {
            foreach (var pop in _elements)
            {
                if(pop == null)
                    continue;
                pop.Delay = e_delayAll;
                UnityEditor.EditorUtility.SetDirty(pop);
            }
        }
        
        [ContextMenu("Set Duration All")]
        public void SetDurationAll()
        {
            foreach (var pop in _elements)
            {
                if(pop == null)
                    continue;
                pop.Duration = e_durationAll;
                UnityEditor.EditorUtility.SetDirty(pop);
            }
        }

        [ContextMenu("Set Ease All")]
        public void SetEaseAll()
        {
            foreach (var pop in _elements)
            {
                var pp = (SimplePopElement)pop;
                if(pp == null)
                    continue;
#if HAS_DOTWEEN
                pp.Ease = e_easeAll;
 #endif              
                UnityEditor.EditorUtility.SetDirty(pop);
            }
        }
#endif
    }
    
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(PopAnimator))]
    public class PopAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as PopAnimator;
            const int width = 130;
            const int height = 30;
            const int spacing = 4;
            GUILayout.Space(20);
            if (GUILayout.Button($"Play", GUILayout.Width(width), GUILayout.Height(height)))
                me.E_Play();
            GUILayout.Space(spacing);
            
            if (GUILayout.Button($"Build from go", GUILayout.Width(width), GUILayout.Height(height)))
                me.E_Build();
            GUILayout.Space(spacing);

            if (GUILayout.Button($"Set Duration",GUILayout.Width(width), GUILayout.Height(height)))
                me.SetDurationAll();   
            GUILayout.Space(spacing);

            if (GUILayout.Button($"Set Delay",GUILayout.Width(width), GUILayout.Height(height)))
                me.SetDelayAll();
            GUILayout.Space(spacing);

            if (GUILayout.Button($"Set Ease", GUILayout.Width(width), GUILayout.Height(height)))
                me.SetEaseAll();   
            GUILayout.Space(spacing);
        }
    }
    #endif
}