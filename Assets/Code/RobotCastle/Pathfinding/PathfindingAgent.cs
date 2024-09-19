#define __DrawPathCells
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace Bomber
{
    public class PathfindingAgent : MonoBehaviour, IAgent
    {
        public enum AgentState { NotMoving, IsMoving, FailedToReach, IsWaitingToPass, Arrived }
        public delegate bool CellMoveCheck(Vector2Int currentCell);

        [SerializeField] private bool _useRb;
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Transform _movable;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private GameObject _pathfindingAgentAnimatorGO;
        private IPathfindingAgentAnimator _pathfindingAgentAnimator;
        private PathfinderAStar _pathfinder;
        private CancellationTokenSource _tokenSource;
        private IMap _map;
        private bool _isMoving;
        private bool _didInit;
        private CellMoveCheck _cellMoveCheck;
        
        public AgentState State { get; private set; }
        public bool IsMoving => _isMoving;
        
        public Vector2Int CurrentCell { get; private set; }
        public IMap Map => _map;
        
        public Transform movable
        {
            get => _movable;
            set => _movable = value;
        }

        public Rigidbody rb
        {
            get => _rb;
            set => _rb = value;
        }

        public GameObject PathfindingAgentAnimatorGO
        {
            get => _pathfindingAgentAnimatorGO;
            set
            {
                _pathfindingAgentAnimatorGO = value;
                if (_pathfindingAgentAnimatorGO != null)
                    _pathfindingAgentAnimator = _pathfindingAgentAnimatorGO.GetComponent<IPathfindingAgentAnimator>();
            }
        }
        
        public IPathfindingAgentAnimator PathfindingAgentAnimator
        {
            get => _pathfindingAgentAnimator;
            set => _pathfindingAgentAnimator = value;
        }
        
        public IFloatGetter SpeedGetter { get; set; }
        
        /// <summary>
        /// Uses ISpeedGetter Instead for continuous update
        /// </summary>
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
        
        private void OnEnable()
        {
            if (_map != null)
            {
                if(_map.ActiveAgents.Contains(this) == false)
                    _map.ActiveAgents.Add(this);
            }
        }

        private void OnDisable()
        {
            Stop();
            if (_map != null)
            {
                _map.ActiveAgents.Remove(this);
            }
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
            if(_cellMoveCheck == null)
                _cellMoveCheck = DefaultCellMoveCheck;
            if(_map.ActiveAgents.Contains(this) == false)
                _map.ActiveAgents.Add(this);
            _pathfinder = new PathfinderAStar(map, new DistanceHeuristic());
            if (_pathfindingAgentAnimatorGO != null && _pathfindingAgentAnimator == null)
                _pathfindingAgentAnimator = _pathfindingAgentAnimatorGO.GetComponent<IPathfindingAgentAnimator>();
        }

        public void SetCellMoveCheck(CellMoveCheck cellMoveCheck)
        {
            if (cellMoveCheck == null)
                _cellMoveCheck = DefaultCellMoveCheck;
            else
                _cellMoveCheck = cellMoveCheck;
        }

        private bool DefaultCellMoveCheck(Vector2Int cell) => true;
        
        public void Stop()
        {
            State = AgentState.NotMoving;
            _isMoving = false;
            _tokenSource?.Cancel();
        }

        public async Task MoveToPosition(Vector3 worldPosition)
        {
            if (_map == null)
            {
                State = AgentState.NotMoving;
                CLog.LogRed("[PathfindingAgent] MAP IS NULL");
                return;
            }
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            _map.GetCellAtPosition(worldPosition, out var targetCellPos, out var cell);
            _rb.isKinematic = false;
            SetCurrentCellFromWorldPosition();
            await MoveToCellAt(targetCellPos, _tokenSource.Token);
        }
        
        public async Task MoveToCellAt(Vector2Int targetCellPos)
        {
            if (_map == null)
            {
                State = AgentState.NotMoving;
                CLog.LogRed("[PathfindingAgent] MAP IS NULL");
                return;
            }
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            _rb.isKinematic = false;
            SetCurrentCellFromWorldPosition();
            await MoveToCellAt(targetCellPos, _tokenSource.Token);
        }
        
        public async Task MoveToCellAt(Vector2Int targetCellPos, CancellationToken token)
        {
            _map.GetCellAtPosition(_movable.position, out var mCellPos, out var mCell);
            // Debug.DrawLine(mCell.worldPosition, mCell.worldPosition + Vector3.up*3f, Color.green, 2f);
            var path = await _pathfinder.FindPath(mCellPos, targetCellPos);
            if (token.IsCancellationRequested)
                return;
            if (!path.success)
            {
                State = AgentState.NotMoving;
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
                State = AgentState.NotMoving;
                return;
            }
            _isMoving = true;
            SetCurrentCellFromWorldPosition();
            _pathfindingAgentAnimator?.OnMovementBegan();
            await MovingThroughPoints(path, token);
            if (!token.IsCancellationRequested)
            {
                if(State != AgentState.FailedToReach)
                    State = AgentState.Arrived;
                _isMoving = false;
                _pathfindingAgentAnimator?.OnMovementStopped();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="cell"></param>
        /// <returns> 0 if free. 1 if busy and moving. 2 if busy and not moving</returns>
        private int CheckIfCellIsFree(Vector2Int cell)
        {
            foreach (var ag in _map.ActiveAgents)
            {
                if (ag == this)
                    continue;
                if (ag.CurrentCell == cell)
                {
                    // var msg = $"[{gameObject.name}] [{cell.ToString()}] Is Already occupied. ";
                    // if (ag.IsMoving)
                    //     msg += "Other is moving, waiting";
                    // else
                    //     msg += "Other is moving, waiting";
                    // CLog.LogWhite(msg);
                    if (ag.IsMoving)
                        return 1;
                    else
                        return 2;
                }
            }
            // CLog.Log($"[{cell.ToString()}] Is free moving");
            return 0;
        }
        
        private async Task MovingThroughPoints(Path path, CancellationToken token)
        {
            const float waitDelay = .5f;
            const float maxWaitTime = 5f;
            for (var i = 1; i < path.points.Count && !token.IsCancellationRequested; i++)
            {
                var targetCell = path.points[i];
                var cellState = CheckIfCellIsFree(targetCell);
                var totalTime = 0f;
                while (cellState == 1)
                {
                    State = AgentState.IsWaitingToPass;
                    totalTime += waitDelay;
                    await Task.Delay((int)(1000 * waitDelay), token);
                    cellState = CheckIfCellIsFree(targetCell);
                    if (totalTime >= maxWaitTime)
                    {
                        CLog.LogRed("Timeout");
                        cellState = 2;
                    }
                }

                switch (cellState)
                {
                    case 0:
                        CurrentCell = targetCell;
                        break; // do nothing, go
                    case 2:
                        State = AgentState.FailedToReach;
                        return;
                }
                State = AgentState.IsMoving;
                var targetWorldPos = _map.Grid[targetCell.x, targetCell.y].worldPosition;
                // CLog.Log($"{gameObject.name} Moving to pos: {targetPos}");
                var rot1 = _movable.rotation;
                var rot2 = Quaternion.LookRotation(targetWorldPos - _movable.position);
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
      
                if (_useRb)
                {
                    var vec = targetWorldPos - transform.position;
                    var d2 = vec.sqrMagnitude;
                    var moveAmount = SpeedGetter.Get() * Time.fixedDeltaTime;
                    while (!token.IsCancellationRequested && d2 > moveAmount * moveAmount)
                    {
                        vec = targetWorldPos - _rb.transform.position;
                        d2 = vec.sqrMagnitude;
                        var force = vec.normalized * SpeedGetter.Get();
                        _rb.velocity = force;
                        await Task.Yield();
                    }
        
                    _rb.velocity = Vector3.zero;
                    _rb.transform.position = targetWorldPos;
                }
                else
                {
                    _movable.rotation = rot2;
                    var totalDistance = (targetWorldPos - transform.position).magnitude;
                    var travelled = 0f;
                    while (!token.IsCancellationRequested && travelled < totalDistance)
                    {
                        var vec = targetWorldPos - transform.position;
                        var vecL = vec.magnitude;
                        var amount = SpeedGetter.Get() * Time.deltaTime;
                        vec *= amount / vecL;
                        travelled += amount;
                        transform.position += vec;
                        await Task.Yield();
                    }
                    if (token.IsCancellationRequested)
                        return;
                    transform.position = targetWorldPos;
                    if (_cellMoveCheck == null)
                    {
                        CLog.LogError($"[{nameof(PathfindingAgent)}] _cellMoveCheck delegate was null!!");
                        _cellMoveCheck = DefaultCellMoveCheck;
                    }
                    var moveToNext = _cellMoveCheck.Invoke(targetCell);
                    if (!moveToNext)
                    {
                        State = AgentState.Arrived;
                        return;
                    }
                }
            }
            State = AgentState.Arrived;
        }

        private void SetCurrentCellFromWorldPosition()
        {
            CurrentCell = _map.GetCellPositionFromWorld(transform.position);
        }
    }
}