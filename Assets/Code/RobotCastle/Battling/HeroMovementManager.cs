using System;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using UnityEngine;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroMovementManager : MonoBehaviour, IPathfindingAgentAnimator
    {
        public HeroView UnitView
        {
            get => _unitView;
            set => _unitView = value;
        }
        
        private const float AngleThreshold = 2;
        [SerializeField] private HeroView _unitView;
        private bool _didSetup;

        public void OnMovementBegan()
        {
            // CLog.LogYellow($"{gameObject.name} MOVEMENT BEGAN");
            _unitView.animator.SetBool(HeroesConstants.Anim_Move, true);
        }

        public void OnMovementStopped()
        {
            // CLog.Log($"{gameObject.name} On Stopped");
            _unitView.animator.SetBool(HeroesConstants.Anim_Move, false);
            _unitView.state.isMoving = false;
        }
   
        public async Task<EPathMovementResult> MoveToCell(Vector2Int pos, CancellationToken token)
        {
            if (!_didSetup)
            {
                CLog.LogRed("Unit Mover NOT setup!!");
                return EPathMovementResult.WasCancelled;
            }
            _unitView.state.isMoving = true;
            return await _unitView.agent.MakeOneStepTowards(pos, token);
        }
        
        public bool CheckIfShouldRotate(Vector2Int cellPos)
        {
            var worldPos = _unitView.agent.Map.GetWorldFromCell(cellPos);
            var rotation = Quaternion.LookRotation(worldPos - transform.position);
            var startRot = transform.rotation;
            var angle = Quaternion.Angle(startRot, rotation);
            return Mathf.Abs(angle) > AngleThreshold;
        }
        
        public async void RotateIfNecessary(Vector2Int cellPos, CancellationToken token, Action callback = null)
        {
            _unitView.state.isMoving = true;
            var worldPos = _unitView.agent.Map.GetWorldFromCell(cellPos);
            var rotation = Quaternion.LookRotation(worldPos - transform.position);
            var startRot = transform.rotation;
            var angle = Quaternion.Angle(startRot, rotation);
            
            if (Mathf.Abs(angle) <= AngleThreshold)
                return;
            var elapsed = 0f;
            var time = angle / _unitView.agent.RotationSpeed;
            while (elapsed < time && !token.IsCancellationRequested)
            {
                transform.rotation = Quaternion.Lerp(startRot, rotation, elapsed / time);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested)
                return;
            transform.rotation = rotation;
            _unitView.state.isMoving = false;
            callback?.Invoke();
        }

        public async Task<bool> RotateIfNecessary(Transform target, CancellationToken token)
        {
            var rotation = Quaternion.LookRotation(target.position - transform.position);
            var startRot = transform.rotation;
            var angle = Quaternion.Angle(startRot, rotation);
            
            if (Mathf.Abs(angle) <= 2)
                return false;
            var elapsed = 0f;
            var time = angle / _unitView.agent.RotationSpeed;
            while (elapsed < time && !token.IsCancellationRequested)
            {
                rotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Lerp(startRot, rotation, elapsed / time);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested)
                return false;
            transform.rotation = rotation;
            return true;
        }
        
        public Quaternion GetRotationToCell(Vector2Int pos)
        {
            var endPos = _unitView.agent.Map.GetWorldFromCell(pos);
            _unitView.agent.Map.GetCellAtPosition(transform.position, out var cellPos, out var cell);
            var startPos = cell.worldPosition;
            return Quaternion.LookRotation(endPos - startPos);
        }

        public void Stop()
        {
            _unitView.agent.Stop();
        }
        
        public void SetupAgent()
        {
            _didSetup = true;
            _unitView.rb.isKinematic = true;
            _unitView.collider.isTrigger = false;
            _unitView.collider.enabled = true;
            _unitView.agent.RotationSpeed = HeroesConstants.RotationSpeed;
            _unitView.agent.SpeedGetter = _unitView.stats.MoveSpeed;
        }

        private void Update()
        {
            if (_didSetup && _unitView != null)
                _unitView.state.currentCell = _unitView.agent.CurrentCell;
        }
    }
}