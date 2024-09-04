using UnityEngine;

namespace SleepDev
{
    public class ToCameraRotator : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private Transform _cameraTr;
        
        
        private void OnEnable()
        {
            _cameraTr = Camera.main.transform;
        }

        private void Update()
        {
            var lookVec = (_target.position - _cameraTr.position);
            lookVec.y = 0;
            _target.rotation = Quaternion.LookRotation(lookVec);
        }
        
        #if UNITY_EDITOR
        [ContextMenu("E_Rotate")]
        public void E_Rotate()
        {
            OnEnable();
            Update();
        }
        #endif
    }
}