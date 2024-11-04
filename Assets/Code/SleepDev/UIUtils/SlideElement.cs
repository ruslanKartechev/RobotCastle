using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class SlideElement : MonoBehaviour
    {
        public float inTime;
        public float outTime;
        public RectTransform rect;
        public Vector2 inPos;
        public Vector2 outPos;
        private Coroutine _working;
        
        public void GoIn()
        {
            Stop();            
            _working = StartCoroutine(Sliding(outPos, inPos, inTime));
        }

        public void GoOut()
        {
            Stop();
            _working = StartCoroutine(Sliding(inPos, outPos, outTime));
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private IEnumerator Sliding(Vector2 from, Vector2 to, float timeTotal)
        {
            var elapsed = Time.deltaTime;
            var time = timeTotal * .78f;
            var t = elapsed / time;
            var maxT = 1.16f;
            while (t < 1f)
            {
                var lerpT = Mathf.Lerp(0f, maxT, t);
                var p = Vector2.LerpUnclamped(from, to, lerpT);
                rect.anchoredPosition = p;
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            elapsed = 0f;
            time = timeTotal * .22f;
            t = elapsed / time;
            while (t < 1f)
            {
                var lerpT = Mathf.Lerp(maxT, 1f, t);
                var p = Vector2.LerpUnclamped(from, to, lerpT);
                rect.anchoredPosition = p;
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            rect.anchoredPosition = to;
        }
        
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            GetRect();
        }

        [ContextMenu("Get Rect")]
        public void GetRect()
        {
            if (rect == null)
                rect = GetComponent<RectTransform>();
        }

        [ContextMenu("** Save In Pos")]
        public void SaveInPos()
        {
            inPos = rect.anchoredPosition;
            Dirty();
        }
        
        [ContextMenu("Set In Pos")]
        public void SetInPos()
        {
            rect.anchoredPosition = inPos;
            Dirty();
        }
        
        
        [ContextMenu("** Save Out Pos")]
        public void SaveOutPos()
        {
            outPos = rect.anchoredPosition;
            Dirty();
        }
        
        [ContextMenu("Set Out Pos")]
        public void SetOutPos()
        {
            rect.anchoredPosition = outPos;
            Dirty();
        }

        private void Dirty()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}