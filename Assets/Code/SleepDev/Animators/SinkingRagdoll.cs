using System.Collections;
using SleepDev.Ragdoll;
using UnityEngine;

namespace SleepDev
{
    public class SinkingRagdoll : MonoBehaviour
    {
        private const string TriggerTag = "SinkTrigger";
        [SerializeField] private IRagdoll _ragdoll;
        [SerializeField] private Rigidbody _rootBone;
        [SerializeField] private SinkingConfig _config;
        [SerializeField] private GameObject _rootGo;
        
        public SinkingConfig Config
        {
            get => _config;
            set => _config = value;
        }
        public bool IsActive { get; set; }
        
#if UNITY_EDITOR
        // private void OnValidate()
        // {
        //     if (Application.isPlaying)
        //         return;
        //     if (_ragdoll == null)
        //     {
        //         _ragdoll = GetComponentInParent<IRagdoll>();
        //         UnityEditor.EditorUtility.SetDirty(this);
        //     }
        // }
#endif
        
        public bool CheckSinkCondition(Collider other)
        {
            if (other.CompareTag(TriggerTag))
            {
                if(_rootBone.position.y < other.transform.position.y)
                    return true;
            }
            return false;
        }
        
        public void Sink()
        {
            IsActive = false;
            // Debug.Break();
            StartCoroutine(Sinking());
        }

        private IEnumerator Sinking()
        {
            _ragdoll.SetGravityAll(false);
            _ragdoll.ZeroVelocities();
            yield return null;
            // _rootBone.isKinematic = true;
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
                y -= v * Time.fixedDeltaTime;
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            y = pos.y;
            time -= floatTime;
            t = 0f;
            while (t < time)
            {
                pos.y = y - v * t - a * t * t;
                tr.position = pos;
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            _rootGo.SetActive(false);
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