#define __DrawPathCells
#define __DrawCurrentCell
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace Bomber
{
    public enum EPathMovementResult { FailedToBuild, IsBlockedOnTheWay, WasCancelled, WasStopped, ReachedEnd }
    public enum AgentState { NotMoving, IsMoving, FailedToReach, IsWaitingToPass, Arrived }

    public class Agent : MonoBehaviour, IAgent
    {
        public delegate bool MoveStepCallback(Vector2Int currentCell);

        public AgentState State { get; private set; }
        public bool IsMoving => State == AgentState.IsMoving;
        
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
        private bool _didInit;
        private MoveStepCallback _moveStepCallback;

        public void UpdateMap(IMap map)
        {
            if (!_didInit)
            {
                InitAgent(map);
                return;
            }
            _map = map;
            SetCurrentCellFromWorldPosition();
        }
        
        public void InitAgent(IMap map)
        {
            if (_didInit)
            {
                // CLog.Log($"[{gameObject.name}] PathfindingAgent already init");
                SetCurrentCellFromWorldPosition();
                return;
            }
            _didInit = true;
            _map = map;
            if(_moveStepCallback == null)
                _moveStepCallback = DefaultStepCallback;
            if(_map.ActiveAgents.Contains(this) == false)
                _map.ActiveAgents.Add(this);
            _pathfinder = new PathfinderAStar(map, new DistanceHeuristic());
            if (_pathfindingAgentAnimatorGO != null && _pathfindingAgentAnimator == null)
                _pathfindingAgentAnimator = _pathfindingAgentAnimatorGO.GetComponent<IPathfindingAgentAnimator>();
            SetCurrentCellFromWorldPosition();
        }

        public void SetCellMoveCheck(MoveStepCallback moveStepCallback)
        {
            if (moveStepCallback == null)
                _moveStepCallback = DefaultStepCallback;
            else
                _moveStepCallback = moveStepCallback;
        }

        private bool DefaultStepCallback(Vector2Int cell) => true;
        
        public void Stop()
        {
            State = AgentState.NotMoving;
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
        
        public async Task<EPathMovementResult> MoveToCellAt(Vector2Int targetCellPos, CancellationToken token)
        {
            var prevMovingState = IsMoving;
            _map.GetCellAtPosition(_movable.position, out var mCellPos, out var mCell);
            var path = await _pathfinder.FindPath(mCellPos, targetCellPos);
            if (token.IsCancellationRequested)
                return EPathMovementResult.FailedToBuild;
            if (!path.success)
            {
                State = AgentState.NotMoving;
                CLog.Log($"[{nameof(Agent)}] Pathfinding to {targetCellPos} FAILED..." );
            }
#if DrawPathCells
            var colors = new List<Color>()
                { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.cyan };
            var color = colors.Random();
            foreach (var cp in path.points)
            {
                var rand = UnityEngine.Random.insideUnitCircle * .15f;
                var p1 = _map.Grid[cp.x, cp.y].worldPosition + new Vector3(rand.x, 0f, rand.y);
                var p2 = p1 + Vector3.up * 2;
                Debug.DrawLine(p1, p2, color, 2f);
            }
            // CLog.LogBlue($"Point_1 {path.points[0]}. Point_end {path.points[^1]}");
#endif
            if (path.points.Count < 2)
            {
                // CLog.LogWhite($"[{nameof(PathfindingAgent)}] path count < 2...");
                State = AgentState.NotMoving;
                return EPathMovementResult.FailedToBuild;
            }
            State = AgentState.IsMoving;
            SetCurrentCellFromWorldPosition();
            if(!prevMovingState)
                _pathfindingAgentAnimator?.OnMovementBegan();
            var result = await MovingThroughPoints(path, token);
            if (!token.IsCancellationRequested)
            {
                State = AgentState.NotMoving;
                _pathfindingAgentAnimator?.OnMovementStopped();
            }
            return result;
        }
        
        
        public async Task<EPathMovementResult> MakeOneStepTowards(Vector2Int targetCellPos, CancellationToken token)
        {
            var alreadyMoving = IsMoving;
            _map.GetCellAtPosition(_movable.position, out var mCellPos, out var mCell);
            var path = await _pathfinder.FindPath(mCellPos, targetCellPos);
            if (token.IsCancellationRequested)
                return EPathMovementResult.FailedToBuild;
            if (!path.success)
            {
                State = AgentState.NotMoving;
                CLog.Log($"[{nameof(Agent)}] Pathfinding to {targetCellPos} FAILED..." );
            }
            if (path.points.Count < 2)
            {
                // CLog.LogWhite($"[{nameof(PathfindingAgent)}] path count < 2...");
                State = AgentState.NotMoving;
                return EPathMovementResult.FailedToBuild;
            }
            State = AgentState.IsMoving;
            SetCurrentCellFromWorldPosition();
            if(!alreadyMoving)
                _pathfindingAgentAnimator?.OnMovementBegan();
            var result = await MoveToCell(path.points[1], token);
            if (!token.IsCancellationRequested)
            {
                State = AgentState.NotMoving;
                _pathfindingAgentAnimator?.OnMovementStopped();
                // SetCurrentCellFromWorldPosition();
            }
            return result;
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
                    var msg = $"[{gameObject.name}] [{cell.ToString()}] Is Already occupied. ";
                    if (ag.IsMoving)
                        msg += "Other is moving, waiting";
                    else
                        msg += "Other IsN'T moving, waiting";
                    CLog.LogWhite(msg);
                    if (ag.IsMoving)
                        return 1;
                    return 2;
                }
            }
            // CLog.Log($"[{cell.ToString()}] Is free moving");
            return 0;
        }
        
        private async Task<EPathMovementResult> MovingThroughPoints(Path path, CancellationToken token)
        {
            // const float waitDelay = .5f;
            // const float maxWaitTime = 5f;
            for (var i = 1; i < path.points.Count && !token.IsCancellationRequested; i++)
            {
                var targetCell = path.points[i];
                var res = await MoveToCell(targetCell, token);
                if (res != EPathMovementResult.ReachedEnd)
                    return res;
                if (_moveStepCallback == null)
                {
                    CLog.LogError($"[{nameof(Agent)}] _cellMoveCheck delegate was null!!");
                    _moveStepCallback = DefaultStepCallback;
                }
                var moveToNext = _moveStepCallback.Invoke(targetCell);
                if (!moveToNext)
                {
                    State = AgentState.Arrived;
                    return EPathMovementResult.WasStopped;
                }
            }
            State = AgentState.Arrived;
            return EPathMovementResult.ReachedEnd;
        }

        private async Task<EPathMovementResult> MoveToCell(Vector2Int targetCell, CancellationToken token)
        {
            var cellState = CheckIfCellIsFree(targetCell);
            if (token.IsCancellationRequested)
                return EPathMovementResult.WasCancelled;
            switch (cellState)
            {
                case 0:
                    CurrentCell = targetCell;
                    break; // do nothing, go
                case 1 or 2:
                    State = AgentState.FailedToReach;
                    return EPathMovementResult.IsBlockedOnTheWay;
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
                return EPathMovementResult.WasCancelled;
      
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
                if (token.IsCancellationRequested)
                    return EPathMovementResult.WasCancelled;
                State = AgentState.Arrived;
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
                    return EPathMovementResult.WasCancelled;
                transform.position = targetWorldPos;
     
            }
            return EPathMovementResult.ReachedEnd;
        }
        

        public void SetCurrentCellFromWorldPosition()
        {
            CurrentCell = _map.GetCellPositionFromWorld(transform.position);
#if UNITY_EDITOR && DrawCurrentCell
            var cellPos = _map.GetWorldFromCell(CurrentCell);
            var mypos = transform.position;
            var height = 3f;
            var time = 3f;
            Debug.DrawLine(cellPos, cellPos + Vector3.up * height, Color.red, time);
            Debug.DrawLine(mypos, mypos + Vector3.up * height, Color.blue, time);
#endif
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

        
    }
}