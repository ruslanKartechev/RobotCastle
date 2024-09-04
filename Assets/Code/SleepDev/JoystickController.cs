using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class JoystickController : MonoBehaviour
    {
        [SerializeField] protected float _sensitivity = 1;
        
        public float Sensitivity
        {
            get => _sensitivity;
            set => _sensitivity = value;
        }
        
        protected float _maxRad = 100;
        protected float _maxRad2;
        
        public float MaxRad
        {
            get => _maxRad;
            set
            {
                _maxRad = value;
                _maxRad2 = value * value;
            } 
        }
        protected Vector2 _position;
        protected Coroutine _working;
        
        public void Reset()
        {
            _position = Vector2.zero;
        }

        public void Move(Vector2 delta)
        {
            _position += delta;
            var magn = _position.magnitude;
            if (magn > _maxRad)
                _position = _position / magn * _maxRad;
        }

        public float Percent() => _position.magnitude / _maxRad;

        public Vector3 GetXZPlane() => new Vector3(_position.x, 0f, _position.y);

        public Vector3 GetScaledXZPlane() => new Vector3(_position.x, 0f, _position.y) / _maxRad;

        public Vector3 GetPos() => _position;

        public void CheckDelta()
        {
            Stop();
            _working = StartCoroutine(CheckingDelta());
        }

        public virtual void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        protected IEnumerator CheckingDelta()
        {
            var pos = Input.mousePosition;
            while (true)
            {
                var nPos = Input.mousePosition;
                Vector2 delta = nPos - pos;
                delta *= Sensitivity;
                Move(delta);
                pos = nPos;
                yield return null;
            }
        }
    }
}