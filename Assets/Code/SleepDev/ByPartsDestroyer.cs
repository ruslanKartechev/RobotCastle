using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class ByPartsDestroyer : MonoBehaviour
    {
        [SerializeField] private bool _useDelay;
        [SerializeField] private List<Part> _parts;

        [System.Serializable]
        public class Part
        {
            public Rigidbody rb;
            public Collider coll;
            public float delay;
            public Transform forceDirection;
            public float force;

            public void Push()
            {
                rb.isKinematic = false;
                coll.enabled = true;
                var forceVec = forceDirection.forward * force;
                rb.AddForce(forceVec, ForceMode.Impulse);
                rb.AddTorque(forceVec, ForceMode.Impulse);
            }
        }
        
        public void BreakAll(Action onEnd)
        {
            if(_useDelay)
                StartCoroutine(Breaking(onEnd));
            else
            {
                foreach (var part in _parts)
                    part.Push();
                onEnd.Invoke();
            }
        }

        private IEnumerator Breaking(Action onEnd)
        {
            foreach (var part in _parts)
            {
                yield return new WaitForSeconds(part.delay);
                part.Push();
            }
            onEnd.Invoke();
        }
        
#if UNITY_EDITOR
        [Space(10)]
        public List<GameObject> e_partsToBuild;
        public float e_forceAll;
        public Transform e_pushDirAll;

        [ContextMenu("BuildParts")]
        public void BuildParts()
        {
            _parts.Clear();
            foreach (var go in e_partsToBuild)
            {
                var rb = go.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = go.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                }
                var coll = go.GetComponent<Collider>();
                if (coll == null)
                {
                    coll = go.AddComponent<BoxCollider>();
                    coll.enabled = false;
                }
                if(rb==null || coll == null)
                    continue;
                var part = new Part()
                {
                    rb = rb,
                    coll = coll,
                    forceDirection = e_pushDirAll ?? go.transform,
                    delay = 0f,
                    force = e_forceAll,
                };
                _parts.Add(part);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("SetForceAll")]
        public void SetForceAll()
        {
            foreach (var part in _parts)
            {
                part.force = e_forceAll;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("SetPushDirAll")]
        public void SetPushDirAll()
        {
            foreach (var part in _parts)
            {
                part.forceDirection = e_pushDirAll;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("E_Destroy")]
        public void E_Destroy()
        {
            BreakAll(() => {});
        }
#endif
        
    }
}