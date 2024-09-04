using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class SinkingPartsController : MonoBehaviour
    {
        [SerializeField] private float _defaultDelay = 2f;
        [SerializeField] private SinkingConfig _config;
        [SerializeField] private List<SinkingAnimator> _animators;
        public List<SinkingAnimator> Animators => _animators;
        
#if UNITY_EDITOR
        public void OnValidate()
        {
            if(_animators.Count == 0)
                GetParts();
        }

        [ContextMenu("GetParts")]
        public void GetParts()
        {
            _animators = MiscUtils.GetFromAllChildren<SinkingAnimator>(transform);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        private void Awake()
        {
            foreach (var aa in _animators)
                aa.Config = _config;
        }
        
        public void ActivateDefault()
        {
            ActivateAfterDelay(_defaultDelay);
        }
        
        public void ActivateAfterDelay(float delay)
        {
            // CLog.Log($"Called sinking, delay {delay}");
            StartCoroutine(Delayed(delay));
        }

        public void Activate()
        {
            foreach (var aa in _animators)
            {
                aa.Config = _config;
                aa.IsActive = true;
            }
        }

        private IEnumerator Delayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            Activate();
        }

    }
}