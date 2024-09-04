using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
#if HAS_DOTWEEN
using DG.Tweening;
#endif

namespace SleepDev
{
    public class SpinWheel : MonoBehaviour
    {
        [SerializeField] private float _mainTime;
        [SerializeField] private float _startSpeed = 500f;
        [SerializeField] private float _endSpeed = 50;
        [SerializeField] private float _overrunAngle = 20;
        [SerializeField] private float _finalSpeed = 10;
        [SerializeField] private AnimationCurve _finalAnimationCurve;
        [Space(10)]
        [SerializeField] private float _loadAngle = 10;
        [SerializeField] private float _loadTime = 1;
        [SerializeField] private float _afterLoadDelay = .15f;
        [SerializeField] private AnimationCurve _loadingCurve;
        [Space(10)] 
        [SerializeField] private Vector3 _normalScale;
        [SerializeField] private float _loadingScale;
        [SerializeField] private float _scaleBackTime;
        #if HAS_DOTWEEN
        [SerializeField] private Ease _scaleEase;
        #endif
        [SerializeField] private Transform _scalable;
        [Space(10)]
        [SerializeField] private List<AngleRange> _ranges;
        [SerializeField] private Transform _rotatable;
        private int _currentIndex;
        private Coroutine _spinning;
        
        public void SpinToIndex(int index, Action rollbackStartCallback, Action stopCallback)
        {
            if (index >= _ranges.Count)
            {
                CLog.Log($"[SpinWheel] {index} is outside range!!");
                return;
            }
            if(_spinning != null)
                StopCoroutine(_spinning);
            _spinning = StartCoroutine(SpinningPositiveDirection(index, rollbackStartCallback, stopCallback));
        }

        public void StopSpinning()
        {
            if(_spinning != null)
                StopCoroutine(_spinning);
        }

        private IEnumerator SpinningPositiveDirection(int targetIndex, Action rollBackCallback, Action stopCallback)
        {
            var elapsed = Time.deltaTime;
            var angle = GetAngle();
            if (angle != 0)
            {
                var firstAngle = angle;
                var turnBackTime = .25f;
                while (elapsed < turnBackTime)
                {
                    SetAngle(Mathf.Lerp(firstAngle, 0f, elapsed / turnBackTime));
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                SetAngle(0f);
            }
            elapsed = Time.deltaTime;
            angle = GetAngle();
            
            var spinStartAngle = angle;
            var loadAngle = spinStartAngle + _loadAngle;
            var scale1 = _normalScale;
            var scale2 = scale1 * _loadingScale;
            while (elapsed < _loadTime)
            {
                var t = elapsed / _loadTime;
                angle = Mathf.Lerp(spinStartAngle, loadAngle, t);
                _scalable.localScale = Vector3.Lerp(scale1, scale2, t);;
                SetAngle(angle);
                elapsed += Time.deltaTime * _loadingCurve.Evaluate(t);
                yield return null;
            }
            _scalable.localScale = scale2;
            SetAngle(loadAngle);
            yield return new WaitForSeconds(_afterLoadDelay);
            #if HAS_DOTWEEN
            _scalable.DOScale(scale1, _scaleBackTime).SetEase(_scaleEase);
            #else
            _scalable.localScale = scale1;
            #endif
            
            angle = GetAngle();
            spinStartAngle = angle;
            var speed = _startSpeed;
            var endCenterAngle = _ranges[targetIndex].center;
            // CLog.LogWhite($"endCenterAngle {endCenterAngle}");
            var endAngle = endCenterAngle + _overrunAngle;
            elapsed = 0f;
            var deltaSpeed = _startSpeed - _endSpeed; 
            var A1 = (.5f * deltaSpeed * _mainTime) + (_endSpeed * _mainTime); // area 1
            var defaultStopAngle = spinStartAngle + A1 % 360f;
            var newStopAngle =  ClampAngle(endAngle); // -_dampingApplyMaxDistance
            var additionalDistance = DistanceCCW(newStopAngle, defaultStopAngle);
            var A2 = A1 + additionalDistance; // area 2
            var time = A2 / (.5f * deltaSpeed + _endSpeed);
            // CLog.LogWhite($"start: {angle}, defaultStopAngle {defaultStopAngle}. EndAngle: {endAngle}. newStopAngle {newStopAngle}. additionalAngle {additionalDistance}");
            // CLog.Log($"Area 1: {A1}. Area 2: {A2}. NewTime: {time}. AdditionalAngle: {additionalDistance}");
            while (elapsed < time)
            {
                speed = Mathf.Lerp(_startSpeed, _endSpeed, elapsed / time);
                angle += speed * Time.deltaTime;
                elapsed += Time.deltaTime;
                SetAngle(angle);
                yield return null;
            }
            angle = GetAngle();
            angle %= 360f;
            var distance = LeftAngleCW(endCenterAngle, angle);
            var startD = distance;
            // CLog.LogWhite($"Running back. Angle: {angle}. Distance: {distance}");
            rollBackCallback?.Invoke();
            while (distance > 0)
            {
                var t = distance / startD;
                speed = _finalSpeed * _finalAnimationCurve.Evaluate(t);
                var diff = speed * Time.deltaTime;
                angle = ClampAngle(angle - diff);
                distance -= diff;
                // CLog.Log($"Angle: {angle}. Distance: {distance}");
                SetAngle(angle);
                yield return null;
            }
            stopCallback?.Invoke();
        }

        /// <summary>
        /// Angle left to go from current-to-target. Going in counter-clock-wise direction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float DistanceCCW(float target, float current)
        {
            var left = (target - current);
            if (left < 0)
                left = 360 + left; // left is negative here
            return left;
        }
        
        /// <summary>
        /// Angle left to go from current-to-target. Going in clock-wise direction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float LeftAngleCW(float target, float current)
        {
            var left = (current - target);
            if (left < 0)
                left = 360 + left; // left is negative here
            return left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetAngle()
        {
            var a = _rotatable.eulerAngles.z;
            if (a < 0)
                a += 360;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAngle(float a)
        {
            var eulers = _rotatable.eulerAngles;
            eulers.z = a;
            _rotatable.eulerAngles = eulers;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float ClampAngle(float a)
        {
            if (a >= 360)
                a -= 360;
            return a;
        }
        

        #if UNITY_EDITOR
        [Space(10)] [Header("Editor")] 
        public int sectionsCount = 5;
        
        [ContextMenu("E_FillRanges")]
        public void E_FillRanges()
        {
            var count = sectionsCount;
            var space = 360f / count;
            var half = space / 2f;
            _ranges = new List<AngleRange>(count);
            var center = 0f;
            for (var i = 0; i < count; i++)
            {
                var min = center - half;
                var max = center + half;
                if (min < 0)
                    min += 360;
                var range = new AngleRange() { min = min, max = max };
                range.center = range.Center;
                _ranges.Add(range);
                center += space;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
        

        [System.Serializable]
        private struct AngleRange
        {
            public float min;
            public float max;
            public float center;

            public float Center
            {
                get
                {
                    if(max > 270 && min < 90)
                        return ((max + min) % 360) * .5f;
                    
                    return (max + min) * .5f;
                }
            }

            public float Width
            {
                get
                {
                    if (max >= min)
                        return max - min;
                    
                    var diff = min - max;
                    if (diff >= 180)
                        diff = 360 - diff;
                    return diff;
                }
            }
        }
    }
}