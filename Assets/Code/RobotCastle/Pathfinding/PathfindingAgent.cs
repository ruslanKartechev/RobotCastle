#define __DrawPathCells
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace Bomber
{
    public class PathfindingAgent : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Transform _movable;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private GameObject _IPathfindingAgentAnimatorGO;

        private IPathfindingAgentAnimator _pathfindingAgentAnimator;
        private PathfinderAStar _pathfinder;
        private CancellationTokenSource _tokenSource;
        private IMap _map;
        private bool _isMoving;
        private bool _didInit;

        public IMap Map => _map;
        
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }
        
        public float RotationSpeed
        {
            get => _rotationSpeed;
            set => _rotationSpeed = value;
        }

        public void InitAgent(IMap map)
        {
            if (_didInit)
            {
                CLog.Log($"[{gameObject.name}] PathfindingAgent already init");
                return;
            }
            _didInit = true;
            
            _map = map;
            _pathfinder = new PathfinderAStar(map, new DistanceHeuristic());
            if (_IPathfindingAgentAnimatorGO != null && _pathfindingAgentAnimator == null)
            {
                _pathfindingAgentAnimator = _IPathfindingAgentAnimatorGO.GetComponent<IPathfindingAgentAnimator>();
            }
        }

        public void Stop()
        {
            _isMoving = false;
            _tokenSource?.Cancel();
        }

        public async Task MoveToPosition(Vector3 worldPosition)
        {
            if (_map == null)
            {
                CLog.LogRed("[PathfindingAgent] MAP IS NULL");
                return;
            }
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            _map.GetCellAtPosition(worldPosition, out var targetCellPos, out var cell);
            _rb.isKinematic = false;
            await MoveToCellAt(targetCellPos, _tokenSource.Token);
        }
        
        public async Task MoveToCellAt(Vector2Int targetCellPos)
        {
            if (_map == null)
            {
                CLog.LogRed("[PathfindingAgent] MAP IS NULL");
                return;
            }
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            _rb.isKinematic = false;
            await MoveToCellAt(targetCellPos, _tokenSource.Token);
        }
        
        public async Task MoveToCellAt(Vector2Int targetCellPos, CancellationToken token)
        {
            _map.GetCellAtPosition(_movable.position, out var mCellPos, out var mCell);
            // Debug.DrawLine(mCell.worldPosition, mCell.worldPosition + Vector3.up*3f, Color.green, 2f);
            var path = await _pathfinder.FindPath(mCellPos, targetCellPos);
            if (token.IsCancellationRequested)
            {
                // CLog.LogRed("token.IsCancellationRequested");
                return;
            }
            if (!path.success) {
                CLog.Log($"[{nameof(PathfindingAgent)}] Pathfinding to {targetCellPos} FAILED..." );
            }
#if DrawPathCells
            foreach (var cp in path.points)
            {
                var p1 = _map.Grid[cp.x, cp.y].worldPosition;
                var p2 = p1 + Vector3.up * 2;
                Debug.DrawLine(p1, p2, Color.blue, 3f);
            }
#endif
            if (path.points.Count < 2)
            {
                // CLog.LogWhite($"[{nameof(PathfindingAgent)}] path count < 2...");
                _isMoving = false;
                return;
            }
            _isMoving = true;
            _pathfindingAgentAnimator?.OnMovementBegan();
            await MovingOnPath(path, token);
            if (!token.IsCancellationRequested)
            {
                _pathfindingAgentAnimator?.OnMovementStopped();
                _isMoving = false;
            }
        }
        
        private void OnDisable()
        {
            Stop();
        }
        
        private async Task MovingOnPath(Path path, CancellationToken token)
        {
            for (var i = 1; i < path.points.Count && !token.IsCancellationRequested; i++)
            {
                var targetPos = _map.Grid[path.points[i].x, path.points[i].y].worldPosition;
                // CLog.Log($"{gameObject.name} Moving to pos: {targetPos}");
                var rot1 = _movable.rotation;
                var rot2 = Quaternion.LookRotation(targetPos - _movable.position);
                var angle = Quaternion.Angle(rot1, rot2);
                var rotTime = angle / _rotationSpeed;
                var elapsed = 0f;
                while (!token.IsCancellationRequested && elapsed < rotTime)
                {
                    _movable.rotation = Quaternion.Lerp(rot1, rot2, elapsed / rotTime);
                    elapsed += Time.deltaTime;
                    await Task.Yield();
                }
                if (token.IsCancellationRequested)
                    return;
                _movable.rotation = rot2;

                var vec = targetPos - _rb.transform.position;
                var d2 = vec.sqrMagnitude;
                var moveAmount = _speed * Time.fixedDeltaTime;
                while (!token.IsCancellationRequested && d2 > moveAmount * moveAmount)
                {
                    vec = targetPos - _rb.transform.position;
                    d2 = vec.sqrMagnitude;
                    var force = vec.normalized * _speed;
                    _rb.velocity = force;
                    await Task.Yield();
                }
                if (token.IsCancellationRequested)
                    return;
                _rb.velocity = Vector3.zero;
                _rb.transform.position = targetPos;
            }
        }

    }
}