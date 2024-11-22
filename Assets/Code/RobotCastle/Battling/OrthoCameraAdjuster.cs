using UnityEngine;

namespace RobotCastle.Battling
{
    public class OrthoCameraAdjuster : MonoBehaviour
    {
        public float GetRatio()
        {
            var r1 = _refResolution.x / _refResolution.y;
            var r2 = (float)Screen.width / Screen.height;
            return r1 / r2;
        }

        public float GetCurrentNormalized() => _camera.orthographicSize / GetRatio();
        
        public float AdjustSize(float size)
        {
            return size * GetRatio();
        }

        public void SetAdjustedSize(float size)
        {
            _camera.orthographicSize = AdjustSize(size);
            // CLog.LogYellow($"Size called {size}, adjusted: {_camera.orthographicSize}");
        }

        public void SetNormal()
        {
            var size = AdjustSize(_normalSize);
            _camera.orthographicSize = size;
        }

        [SerializeField] private Camera _camera;
        [SerializeField] private Vector2 _refResolution;
        [SerializeField] private float _normalSize = 9;
        [SerializeField] private bool _autoAdjustOnEnable;

        private void OnEnable()
        {
            if (_autoAdjustOnEnable)
            {
                SetNormal();
            }
        }
      
        private void Update()
        {
            // var size = AdjustSize(_normalSize);
            // _camera.orthographicSize = size;
        }
    }
}