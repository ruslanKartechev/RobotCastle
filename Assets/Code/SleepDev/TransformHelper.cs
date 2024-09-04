using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    public class TransformHelper : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private Transform _copyFrom;
        [SerializeField] private Transform _lookAt;

        public void SwitchPlaces()
        {
            if (_copyFrom == null)
            {
                Debug.LogError($"_copyFrom == null");
                return;
            }
            var mPos = transform.position;
            var mRot = transform.rotation;
            var mScale = transform.localScale;
            
            transform.SetPositionAndRotation(_copyFrom.position, _copyFrom.rotation);
            transform.localScale = _copyFrom.localScale;
            
            _copyFrom.SetPositionAndRotation(mPos, mRot);
            _copyFrom.localScale = mScale;
         
            UnityEditor.EditorUtility.SetDirty(transform);
            UnityEditor.EditorUtility.SetDirty(_copyFrom);
        }
        
        public void LookAt()
        {
            if (_lookAt == null)
            {
                Debug.LogError($"_lookAt == null");
                return;
            }
            transform.rotation = Quaternion.LookRotation(_lookAt.position - transform.position);
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void CopyPosRot()
        {
            if (_copyFrom == null)
            {
                Debug.LogError($"_copyFrom == null");
                return;
            }
            transform.SetPositionAndRotation(_copyFrom.position, _copyFrom.rotation);
        }
        
        public void ZeroLocalPosRot()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void OneScale()
        {
            transform.localScale = Vector3.one;
        }
        
        public void LookOpposite()
        {
            if (_lookAt == null)
            {
                Debug.LogError($"_lookAt == null");
                return;
            }
            transform.rotation = Quaternion.LookRotation(-(_lookAt.position - transform.position));
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        
#endif
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(TransformHelper))]
    public class TransformHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as TransformHelper;
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Zero Pos Rot", GUILayout.Width(120)))
            {
                me.ZeroLocalPosRot();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            if (GUILayout.Button("One Scale", GUILayout.Width(120)))
            {
                me.OneScale();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Look at", GUILayout.Width(120)))
            {
                me.LookAt();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            if (GUILayout.Button("Look opposite", GUILayout.Width(120)))
            {
                me.LookOpposite();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy", GUILayout.Width(120)))
            {
                me.CopyPosRot();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            if (GUILayout.Button("Switch Points", GUILayout.Width(120)))
            {
                me.SwitchPlaces();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
        }
    }
    #endif
}