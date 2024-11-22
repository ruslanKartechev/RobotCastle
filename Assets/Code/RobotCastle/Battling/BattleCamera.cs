using System;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleCamera : MonoBehaviour
    {
        private const float thresholdMinDistance = .1f;

        public int SlideBlockers
        {
            get => _slideBlockers;
            set
            {
                _slideBlockers = value;
                if(_slideBlockers < 0)
                    _slideBlockers = 0;
            } 
        }

        public float MoveTime
        {
            get => _moveTime;
            set => _moveTime = value;
        }

        /// <summary>
        /// 0 - merge, 1 - battle
        /// </summary>
        public int PositionIndex => _positionIndex;
        
        [SerializeField] private Camera _camera;
        [SerializeField] private float _moveTime;
        [SerializeField] private Transform _mergePoint;
        [SerializeField] private Transform _battlePoint;
        [SerializeField] private float _cameraSizeNormal;
        [SerializeField] private float _cameraSizeWide;
        [SerializeField] private float _cameraSizeMerge;
        [SerializeField] private AnimationCurve _sizeAnimationCurve;
        [SerializeField] private float _sizeAnimationTime = 1;
        [SerializeField] private float _moveBackTime = 1; 
        [SerializeField] private float _moveBackDelay = 1;
        [SerializeField] private AnimationCurve _moveBackAnimationCurve;
        [SerializeField] private OrthoCameraAdjuster _orthoCameraAdjuster;
        private CancellationTokenSource _tokenSource;
        private int _slideBlockers;
        private int _positionIndex;
    
        public void AllowPlayerInput(bool allowed)
        {
            if(allowed)
                ServiceLocator.Get<GameInput>().OnSlideMain += OnSlide;
            else
                ServiceLocator.Get<GameInput>().OnSlideMain -= OnSlide;
        }

        private void OnSlide(Vector3 vec)
        {
            if (SlideBlockers > 0)
                return;
            if (Mathf.Abs(vec.y) > Math.Abs(vec.x))
            {
                if (vec.y < 0)
                {
                    MoveAndSizeToBattlePoint();
                    var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
                    if(battleUI)
                        battleUI.SetMainAreaLowerPos();
                }
                else if (vec.y > 0)
                {
                    MoveAndSizeToMergePoint();
                    var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
                    if(battleUI)
                        battleUI.SetMainAreaUpPos();
                }
            }
        }

        public void SetMergePoint()
        {
            _positionIndex = 0;
            transform.position = _mergePoint.position;
        }

        public void SetBattlePoint()
        {
            _positionIndex= 1;
            transform.position = _battlePoint.position;
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
        }

        public void MoveAndSizeToBattlePoint()
        {
            if ((transform.position - _battlePoint.position).magnitude <= thresholdMinDistance)
                return;
            Stop();
            MovingAndScalingToBattle(_tokenSource.Token);
        }

        public void MoveAndSizeToMergePoint()
        {
            if ((transform.position - _mergePoint.position).magnitude <= thresholdMinDistance)
                return;
            Stop();
            MovingAndScalingToMerge(_tokenSource.Token);
        }

        public async Task MovingAndScalingToMerge(CancellationToken token)
        {
            _positionIndex = 0;
            await Task.WhenAll(MovingCamera(transform.position, _mergePoint.position, _moveTime, _moveBackAnimationCurve, token),
                ChangeCameraSize(_orthoCameraAdjuster.GetCurrentNormalized(), _cameraSizeMerge, _moveTime, token));
        }
        
        public async Task MovingAndScalingToBattle(CancellationToken token)
        {
            _positionIndex = 1;
            await Task.WhenAll(MovingCamera(transform.position, _battlePoint.position, _moveTime, _moveBackAnimationCurve, token),
                ChangeCameraSize(_orthoCameraAdjuster.GetCurrentNormalized(), _cameraSizeNormal, _moveTime, token));
        }
        
        public async Task PlayStartAnimation(CancellationToken token)
        {
            _positionIndex = 1;
            await AnimateCameraSize(token);
            if (token.IsCancellationRequested) return;
            await HeroesManager.WaitGameTime(_moveBackDelay, token);
            if (token.IsCancellationRequested) return;
            
            _positionIndex = 0;
            var t1 = MovingCamera(_battlePoint.position, _mergePoint.position, _moveBackTime, _moveBackAnimationCurve, token );
            var t2 = ChangeCameraSize(_camera.orthographicSize, _cameraSizeMerge, _moveTime, token);
            await Task.WhenAll(t1, t2);
        }

        public async Task MovingCamera(Vector3 pos1, Vector3 pos2, float time, AnimationCurve curve, CancellationToken token)
        {
            var elapsed = 0f;
            var t = 0f;
            while (t <= 1f && !token.IsCancellationRequested)
            {
                transform.position = Vector3.Lerp(pos1, pos2, t);
                elapsed += Time.deltaTime * curve.Evaluate(t);
                t = elapsed / time;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            transform.position = pos2;
        }

        public async Task ChangeCameraSize(float size1, float size2, float time, CancellationToken token)
        {
            var elapsed = 0f;
            var t = 0f;
            while (t <= 1f && !token.IsCancellationRequested)
            {
                var s = Mathf.Lerp(size1, size2, t);
                SetCamSize(s);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            SetCamSize(size2);
        }
        
        public async Task AnimateCameraSize(CancellationToken token)
        {
            var elapsed = 0f;
            var time = _sizeAnimationTime;
            var t = 0f;
            var size1 = _cameraSizeWide;
            var size2 = _cameraSizeNormal;
            
            while (t <= 1f && !token.IsCancellationRequested)
            {
                var s = Mathf.LerpUnclamped(size1, size2, _sizeAnimationCurve.Evaluate(t));
                SetCamSize(s);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            SetCamSize(size2);
        }

        private void SetCamSize(float size)
        {
            _orthoCameraAdjuster.SetAdjustedSize(size);
        }
    }
}