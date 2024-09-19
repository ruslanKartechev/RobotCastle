using System.Threading;
using System.Threading.Tasks;
using Bomber;
using UnityEngine;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroMovementManager : MonoBehaviour, IPathfindingAgentAnimator, IMovingUnit
    {
        [SerializeField] private HeroView _unitView;
        private bool _didSetup;

        public Vector2Int TargetCell { get; set; }
        
        public bool IdleOnStop { get; set; } = true;

        public HeroView UnitView
        {
            get => _unitView;
            set => _unitView = value;
        }

        public void SetNullTargetCell() => TargetCell = new Vector2Int(-1, -1);

        public void OnMovementBegan()
        {
            _unitView.animator.SetBool(HeroesConfig.AnimId_Move, true);
        }

        public void OnMovementStopped()
        {
            _unitView.animator.SetBool(HeroesConfig.AnimId_Move, false);
        }

        public async Task MoveToCell(Vector2Int pos, CancellationToken token)
        {
            if (!_didSetup)
            {
                CLog.LogRed("Unit Mover NOT setup!!");
                return;
            }
            await _unitView.agent.MoveToCellAt(pos, token);
        }
        
        public void MoveToCell(Vector2Int pos)
        {
            if (!_didSetup)
            {
                CLog.LogRed("Unit Mover NOT setup!!");
                return;
            }
            _unitView.agent.MoveToCellAt(pos);
        }
        
        public async Task<bool> RotateIfNecessary(Vector2Int cellPos, CancellationToken token)
        {
            var worldPos = _unitView.agent.Map.GetWorldFromCell(cellPos);
            var rotation = Quaternion.LookRotation(worldPos - transform.position);
            var startRot = transform.rotation;
            var angle = Quaternion.Angle(startRot, rotation);
            
            if (Mathf.Abs(angle) <= 2)
                return false;
            var elapsed = 0f;
            var time = angle / _unitView.agent.RotationSpeed;
            while (elapsed < time && !token.IsCancellationRequested)
            {
                transform.rotation = Quaternion.Lerp(startRot, rotation, elapsed / time);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested)
                return false;
            transform.rotation = rotation;
            return true;
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
            _unitView.agent.RotationSpeed = 800;
            _unitView.agent.SpeedGetter = _unitView.Stats.MoveSpeed;
        }

        public void SetIdle()
        {
            _unitView.rb.isKinematic = true;
            _unitView.collider.isTrigger = true;
            _unitView.collider.enabled = true;
            OnMovementStopped();
        }


    }
}