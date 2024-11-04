using UnityEngine;
using UnityEngine.Animations;

namespace RobotCastle.Utils
{
    public class RotationSpringAnimator : SpringAnimatorListener
    {
        [SerializeField] private float _magnitudeAbs;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Axis _axis = Axis.Y;
        private Vector3 _rotAxis;

        public override void OnStarted()
        {
            switch (_axis)
            {
                case Axis.X:
                    _rotAxis = new Vector3(1,0,0);
                    break;
                case Axis.Y:
                    _rotAxis = new Vector3(0,1,0);
                    break;       
                case Axis.Z:
                    _rotAxis = new Vector3(0,0,1);
                    break;
                default:
                    _rotAxis = new Vector3(0,1,0);
                    break;                    
            }
            _rect.localEulerAngles = Vector3.zero;
        }

        public override void OnUpdated(float val)
        {
            var sign = Mathf.Sign(val);
            var angle = sign * Mathf.Lerp(0f, _magnitudeAbs, val * sign);
            _rect.localEulerAngles = _rotAxis * angle;
        }

        public override void OnStopped()
        {
        }
    }
    
}