using UnityEngine;

namespace SleepDev
{
    public class RectDrawer : MonoBehaviour
    {
        [SerializeField] private RectArea _rectArea;

        public RectArea Area
        {
            get => _rectArea;
            set => _rectArea = value;
        }
        
        #if UNITY_EDITOR
        [SerializeField] private bool _doDraw;
        [SerializeField] private Color _color;
        [SerializeField] private bool _drawCorners;
        [SerializeField] private float _cornersSize;


        private void OnDrawGizmos()
        {
            if(_doDraw)
                Draw();
        }

        public void Draw()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = _color;
            Gizmos.DrawLine(_rectArea.UpLeftCorner(), _rectArea.UpRightCorner());
            Gizmos.DrawLine(_rectArea.UpRightCorner(), _rectArea.DownRightCorner());
            Gizmos.DrawLine(_rectArea.DownRightCorner(), _rectArea.DownLeftCorner());
            Gizmos.DrawLine(_rectArea.DownLeftCorner(), _rectArea.UpLeftCorner());
            if (_drawCorners)
            {
                Gizmos.DrawSphere(_rectArea.UpLeftCorner(), _cornersSize);
                Gizmos.DrawSphere(_rectArea.UpRightCorner(), _cornersSize);
                Gizmos.DrawSphere(_rectArea.DownRightCorner(), _cornersSize);
                Gizmos.DrawSphere(_rectArea.DownLeftCorner(), _cornersSize);
            }
            
            Gizmos.color = oldColor;
            
        }
        #endif
    }
}