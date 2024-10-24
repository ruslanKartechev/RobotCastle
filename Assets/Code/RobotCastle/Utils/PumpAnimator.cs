using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace RobotCastle.Utils
{
    public class PumpAnimator : MonoBehaviour
    {

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rect == null)
            {
                _rect = gameObject.GetComponent<RectTransform>();
                UnityEditor.EditorUtility.SetDirty(this);
            } 
        }
#endif

        private void OnEnable()
        {
            if (_startOnEnable)
                Begin();
        }

        private void OnDisable()
        {
            if (_started)
                Stop();
        }

        [ContextMenu("Begin")]
        public void Begin()
        {
            if(_startOnEnable)
                Stop();
            _startOnEnable = true;
            _sequence = DOTween.Sequence();
            foreach (var data in _pumpData)
            {
                _sequence.Append(_rect.DOPunchScale(_scaleNormal * data.scale, data.time, 1));
            }
            _sequence.SetDelay(_cycleDelay);
            _sequence.SetLoops(-1);
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            _startOnEnable = false;
            _sequence?.Kill();
        }
        
        [SerializeField] private bool _startOnEnable;
        [SerializeField] private float _cycleDelay = 1f;
        [SerializeField] private Vector3 _scaleNormal = Vector3.one;
        [SerializeField] private List<PumpData> _pumpData;
        [SerializeField] private RectTransform _rect;
        private bool _started;
        private Sequence _sequence;

        [System.Serializable]
        public class PumpData
        {
            public float scale;
            public float time;
        }
    }
}