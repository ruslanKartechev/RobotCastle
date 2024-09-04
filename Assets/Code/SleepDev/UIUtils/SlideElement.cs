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
        
        private IEnumerator Sliding(Vector2 from, Vector2 to, float time)
        {
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (true)
            {
                var p = Vector2.Lerp(from, to ,t);
                rect.anchoredPosition = p;
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
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