using System.Linq;
using UnityEngine;

namespace SleepDev
{
    public class RagdollCopier : MonoBehaviour
    {
        [SerializeField] private Transform _copyToRoot;
        [SerializeField] private Ragdoll.Ragdoll _ragdoll;

        private void OnValidate()
        {
            if (_copyToRoot == null)
                _copyToRoot = transform;
            if(_ragdoll == null)
                _ragdoll = MiscUtils.GetFromAllChildren<Ragdoll.Ragdoll>(_copyToRoot).FirstOrDefault();
        }

        [ContextMenu("Copy")]
        public void Copy()
        {
            CopyAllRigidbodies();
            CopyAllColliders();
            CopyAllJoints();
        }

        private void CopyAllRigidbodies() 
        {
            foreach (var part in _ragdoll.parts)
            {
                var go = part.rb.gameObject;
                var toGo = GetByName(_copyToRoot, go.name);
                if (toGo == null)
                {
                    Debug.Log($"GO {go.name} NOT FOUND !!");
                    return;
                }
                CopyRb(go, toGo.gameObject);
                
            }
        }
        
        private void CopyAllColliders() 
        {
            foreach (var part in _ragdoll.parts)
            {
                var go = part.rb.gameObject;
                var toGo = GetByName(_copyToRoot, go.name);
                if (toGo == null)
                {
                    Debug.Log($"GO {toGo.name} NOT FOUND !!");
                    return;
                }
                CopyCollider(go, toGo.gameObject);
            }
        }
        
                
        private void CopyAllJoints() 
        {
            foreach (var part in _ragdoll.parts)
            {
                var go = part.rb.gameObject;
                var toGo = GetByName(_copyToRoot, go.name);
                if (toGo == null)
                {
                    Debug.Log($"GO {toGo.name} NOT FOUND !!");
                    return;
                }
                CopyJoint(go, toGo.gameObject);
            }
        }

        private void CopyRb(GameObject from, GameObject to)
        {
            var rb = from.GetComponent<Rigidbody>();
            if (rb == null)
                return;
            var toRb = GetOrAdd<Rigidbody>(to);
            toRb.mass = rb.mass;
            toRb.constraints = rb.constraints;
            toRb.isKinematic = rb.isKinematic;
            toRb.useGravity = rb.useGravity;
            toRb.drag = rb.drag;
            toRb.angularDrag = rb.angularDrag;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(toRb);     
#endif
        }

        private void CopyJoint(GameObject from, GameObject to)
        {
            var joint = from.GetComponent<CharacterJoint>();
            if (joint == null)
                return;
            var toJoint = GetOrAdd<CharacterJoint>(to);
            var connectedFrom = joint.connectedBody;
            if (connectedFrom != null)
            {
                var connectedTo = GetByName(_copyToRoot, connectedFrom.gameObject.name);
                var toRb = connectedTo.GetComponent<Rigidbody>();
                toJoint.connectedBody = toRb;
            }
            toJoint.enableProjection = joint.enableProjection;
            toJoint.projectionAngle = joint.projectionAngle;
            toJoint.projectionDistance = joint.projectionDistance;
            toJoint.swing1Limit = joint.swing1Limit;
            toJoint.swing2Limit = joint.swing2Limit;
            toJoint.swingAxis = joint.swingAxis;
            toJoint.lowTwistLimit = joint.lowTwistLimit;
            toJoint.highTwistLimit = joint.highTwistLimit;
            toJoint.anchor = joint.anchor;
            toJoint.axis = joint.axis;
            toJoint.swingAxis = joint.swingAxis;
            toJoint.swing1Limit = joint.swing1Limit;
            toJoint.swing2Limit = joint.swing2Limit;
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(toJoint);     
            #endif
        }

        private void CopyCollider(GameObject from, GameObject to)
        {
            var coll = from.GetComponent<Collider>();
            if (coll == null)
                return;
            switch (coll)
            {
                case BoxCollider box:
                {
                    var comp = GetOrAdd<BoxCollider>(to);
                    comp.enabled = box.enabled;
                    comp.center = box.center;
                    comp.size = box.size;
                    comp.isTrigger = box.isTrigger;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(comp);     
#endif
                }
                    break;
                case SphereCollider sphere:
                {
                    var comp = GetOrAdd<SphereCollider>(to);
                    comp.enabled = sphere.enabled;
                    comp.center = sphere.center;
                    comp.radius = sphere.radius;
                    comp.isTrigger = sphere.isTrigger;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(comp);     
#endif
                }
                    break;
                case CapsuleCollider capsule:
                {
                    var comp = GetOrAdd<CapsuleCollider>(to);
                    comp.enabled = capsule.enabled;
                    comp.center = capsule.center;
                    comp.direction = capsule.direction;
                    comp.height = capsule.height;
                    comp.radius = capsule.radius;
                    comp.isTrigger = capsule.isTrigger;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(comp);     
#endif
                }
                    break;

            }
        }

        public T GetOrAdd<T>(GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp == null)
                comp = go.AddComponent<T>();
            return comp;
        }
        
        private Transform GetByName(Transform root, string name)
        {
            var count = root.childCount;
            if (count == 0)
                return null;
            if (name == root.name)
                return root;
            for (var i = 0; i < count; i++)
            {
                var cc = GetByName(root.GetChild(i), name);
                if (cc != null)
                    return cc;
            }
            return null;
        }
    }
}