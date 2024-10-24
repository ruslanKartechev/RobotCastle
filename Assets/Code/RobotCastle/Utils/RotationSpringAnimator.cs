using UnityEngine;

namespace RobotCastle.Utils
{
    public class RotationSpringAnimator : SpringAnimatorListener
    {
        [SerializeField] private float _magnitudeAbs;
        [SerializeField] private RectTransform _rect;

        public override void OnStarted()
        {
            _rect.localEulerAngles = Vector3.zero;
        }

        public override void OnUpdated(float val)
        {
            var sign = Mathf.Sign(val);
            var y = sign * Mathf.Lerp(0f, _magnitudeAbs, val * sign);
            _rect.localEulerAngles = new Vector3(0, y, 0);
        }

        public override void OnStopped()
        {
        }
    }
    
}