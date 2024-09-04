using UnityEngine;

namespace SleepDev
{
    public class ToCameraRotator3D : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private Transform _cameraTr;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_target == null && Application.isPlaying==false)
            {
                _target = transform;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        private void OnEnable()
        {
            _cameraTr = Camera.main.transform;
        }

        private void Update()
        {
            // var lookVec = (_target.position - _cameraTr.position);
            _target.rotation = Quaternion.LookRotation(_cameraTr.forward);
        }

        #if UNITY_EDITOR
        [ContextMenu("Rotate to camera")]
        public void Rotate()
        {
            _cameraTr = Camera.main.transform;
            Update();
        }
        #endif
    }
}