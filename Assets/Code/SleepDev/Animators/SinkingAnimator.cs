using System;
using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class SinkingAnimator : MonoBehaviour
    {
        private const string TriggerTag = "SinkTrigger";
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;

        public Rigidbody Rb => _rb;
        
        public Collider Coll => _collider;
        
        public SinkingConfig Config { get; set; }
        public bool IsActive { get; set; }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();
            if (_collider == null)
                _collider = GetComponent<Collider>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        
        public void Sink()
        {
            _rb.isKinematic = true;
            _collider.enabled = false;
            IsActive = false;
            StartCoroutine(Sinking());
        }

        public bool CheckSinkCondition(Collider other)
        {
            if (other.CompareTag(TriggerTag))
            {
                if(_rb.position.y < other.transform.position.y)
                    return true;
            }
            return false;
        }

        private IEnumerator Sinking()
        {
            var tr = transform;
            var t = 0f;
            var pos = tr.position;
            var time = Config.sinkingTime;
            var v = Config.sinkingV;
            var a = Config.sinkingA;
            var y = pos.y;
            var omega = Config.sinkingFloatOmega;
            var floatTime = Config.sinkingFloatTime.RandomInVec();
            var magn = Config.sinkingFloatMagn.RandomInVec();
            while (t < floatTime)
            {
                pos.y = y + Mathf.Sin(t * omega) * magn;
                tr.position = pos;
                y -= v * Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }

            y = pos.y;
            time -= floatTime;
            t = 0f;
            while (t < time)
            {
                pos.y = y - v * t - a * t * t;
                tr.position = pos;
                t += Time.deltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsActive)
                return;
            if(CheckSinkCondition(other))
                Sink();
        }
    }
}