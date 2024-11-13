using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using RobotCastle.Core;
using UnityEngine;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroMovementManager : MonoBehaviour, IPathfindingAgentAnimator, IAgent
    {
        public HeroComponents UnitView
        {
            get => _unitView;
            set => _unitView = value;
        }

        public IMap Map => _map;

        public bool IsMoving
        {
            get => _isMoving;
            set
            {
                _isMoving = value;
                _unitView.state.isMoving = value;
            }
        }
        public Vector2Int CurrentCell
        {
            get => _cell;
            set
            {
                _cell = value;
                _unitView.state.currentCell = value;
            }
        }

        public Vector2Int TargetCell
        {
            get => _unitView.state.targetMoveCell;
            set => _unitView.state.targetMoveCell = value;
        }
        
        private const float AngleThreshold = 2;

        [SerializeField] private HeroComponents _unitView;
        private Rigidbody _rb;
        private PathfinderAStar _pathfinder;
        private CancellationTokenSource _tokenSource;
        private IMap _map;
        private IFloatGetter _speedGetter;
        private float _rotSpeed;
        private bool _didSetup;
        private bool _isMoving;
        private Vector2Int _cell; 
        
        private static List<Color> colors 
            = new List<Color>() { Color.blue, Color.red, Color.yellow, Color.magenta};
        private Color _dbgColor;
        private Vector3 _offset;

        public void InitAgent(IMap map)
        {
            _rotSpeed  = HeroesConstants.RotationSpeed;
            _speedGetter = _unitView.stats.MoveSpeed;
            if (_map != map || !_didSetup)
            {
                _map = map;
                if(_map.ActiveAgents.Contains(this) == false)
                    _map.ActiveAgents.Add(this);
                _didSetup = true;
                if(_pathfinder == null)
                    _pathfinder = new PathfinderAStar(_map, new DistanceHeuristic());
                else
                    _pathfinder.Map = _map;
                _dbgColor = colors.Random();
                var r = UnityEngine.Random.insideUnitCircle;
                _offset = new Vector3(r.x, 0, r.y) * .2f;
            }
            SyncCellToWorldPos();
        }
        
        public void OnMovementBegan()
        {
            var wasMoving = IsMoving;
            IsMoving = true;
            if (wasMoving) return;
            // CLog.LogGreen($"{gameObject.name} On Began");
            _unitView.animator.SetBool(HeroesConstants.Anim_Move, true);
        }

        public void OnMovementStopped()
        {
            var wasMoving = IsMoving;
            IsMoving = false;
            if (!wasMoving) return;
            SyncCellToWorldPos();
            _unitView.state.SetTargetCellToSelf();
            // CLog.LogRed($"{gameObject.name} On Stopped");
            _unitView.animator.SetBool(HeroesConstants.Anim_Move, false);
        }

        public void MoveToEnemy(IHeroController enemy, 
            Action stepCallback, Action endCallback)
        {
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            MovingToEnemy(enemy, _tokenSource.Token, stepCallback, endCallback);
        }
        
        public async Task<EPathMovementResult> MovingToEnemy(IHeroController enemy, 
            CancellationToken token, Action stepCallback, Action endCallback)
        {
            SyncCellToWorldPos();
            OnMovementBegan();
            
            var movable = transform;
            var myCellPos = _cell;
            var originalEnemyCell = enemy.Components.state.currentCell;
            // CLog.Log($"[{_unitView.gameObject.name}] Pathfinding start");
            var path = await _pathfinder.FindPath(myCellPos, originalEnemyCell);
            if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
            if (!path.success)
            {
                // CLog.LogRed($"[{nameof(HeroMovementManager)}] Path failed: from ({myCellPos}), to ({originalEnemyCell})");
                await Task.Yield();
                if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
                stepCallback?.Invoke();
                if(token.IsCancellationRequested) return EPathMovementResult.WasCancelled;

                await HeroesManager.WaitGameTime(.2f, token);
                if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;

                MoveToEnemy(enemy, stepCallback, endCallback);
                return EPathMovementResult.FailedToBuild;
            }
            if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
            
            if (path.points.Count < 2)
            {
                await Task.Yield();
                if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
                stepCallback?.Invoke();
                if(token.IsCancellationRequested) return EPathMovementResult.WasCancelled;

                CLog.LogRed($"[{nameof(HeroMovementManager)}] path count < 2...");
                return EPathMovementResult.FailedToBuild;
            }
       
            const int CheckEnemyPosIterationsCount = 2;
            for (var i = 1; i < path.points.Count && !token.IsCancellationRequested; i++)
            {
                if (i % CheckEnemyPosIterationsCount == 0)
                {
                    if (originalEnemyCell != enemy.Components.state.currentCell)
                    {
                        // CLog.Log($"[{_unitView.gameObject.name}] Enemy moved starting over");
                        MoveToEnemy(enemy, stepCallback, endCallback);
                        return EPathMovementResult.WasCancelled;
                    }
                }    
                var targetCell =  path.points[i];
                var targetWorldPos = _map.Grid[targetCell.x, targetCell.y].worldPosition;

#region Rotating
                var rot1 = movable.rotation;
                var rot2 = Quaternion.LookRotation(targetWorldPos - movable.position);
                var angle = Quaternion.Angle(rot1, rot2);
                var rotTime = angle / _rotSpeed;
                var elapsed = 0f;
                while (!token.IsCancellationRequested && elapsed < rotTime)
                {
                    movable.rotation = Quaternion.Lerp(rot1, rot2, elapsed / rotTime);
                    elapsed += Time.deltaTime;
                    await Task.Yield();
                }
                if (token.IsCancellationRequested) return EPathMovementResult.WasStopped;
                movable.rotation = rot2;
#endregion

#region EnemyDetection

                var it = 0;
                var itMax = 100;
                var blocked = false;
                do
                {
                    it++;
                    blocked = false;
                    for (var ind = _map.ActiveAgents.Count - 1; ind >= 0; ind--)
                    {
                        var agent = _map.ActiveAgents[ind];
                        if (agent == this)
                            continue;
                        if (agent.CurrentCell == targetCell || agent.TargetCell == targetCell)
                        {
                            stepCallback?.Invoke();
                            if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
                            if (agent.IsMoving && agent.TargetCell != targetCell)
                            {
                                CLog.Log($"[{gameObject.name}] Is blocked my moving agent");
                                blocked = true;
                                await HeroesManager.WaitGameTime(.2f, token);
                                if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
                            }
                            else
                            {
                                CLog.LogRed($"[{gameObject.name}] Is blocked by STANDING agent, starting over");
                                await Task.Yield();
                                if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
                                MoveToEnemy(enemy, stepCallback, endCallback);
                                return EPathMovementResult.IsBlockedOnTheWay;
                            }
                            break;
                        }
                        it++;
                    }
                } while (blocked && !token.IsCancellationRequested && _map.ActiveAgents.Count > 0 && it < itMax);
                if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;

#endregion

#region Moving
                TargetCell = targetCell;
                var totalDistance = (targetWorldPos - movable.position).magnitude;
                var travelled = 0f;
                // CLog.LogGreen($"[{gameObject.name}] Moving to cell {targetCell}, world: {targetWorldPos}. Speed: {_speedGetter.Get()}");
                while (!token.IsCancellationRequested && travelled < totalDistance)
                {
                    var vec = targetWorldPos - movable.position;
                    var vecL = vec.magnitude;
                    var amount = _speedGetter.Get() * Time.deltaTime;
                    vec *= amount / vecL;
                    travelled += amount;
                    movable.position += vec;
                    await Task.Yield();
                }
                if (token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
                movable.position = targetWorldPos;
                CurrentCell = targetCell;
#endregion
                stepCallback?.Invoke();
                if(token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
            }
            if(token.IsCancellationRequested) return EPathMovementResult.WasCancelled;
            OnMovementStopped();
            endCallback?.Invoke();
            return EPathMovementResult.ReachedEnd;
        }

        public async void RotateIfNecessary(Vector2Int cellPos, CancellationToken token, Action callback = null)
        {
            _unitView.state.isMoving = true;
            var worldPos = _map.GetWorldFromCell(cellPos);
            var rotation = Quaternion.LookRotation(worldPos - transform.position);
            var startRot = transform.rotation;
            var angle = Quaternion.Angle(startRot, rotation);
            
            if (Mathf.Abs(angle) <= AngleThreshold)
                return;
            var elapsed = 0f;
            var time = angle / _rotSpeed;
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
            var vec = target.position - transform.position;
            if (vec == Vector3.zero)
                return false;
            var rotation = Quaternion.LookRotation(vec);
            var startRot = transform.rotation;
            var angle = Quaternion.Angle(startRot, rotation);
            
            if (Mathf.Abs(angle) <= 2)
                return false;
            var elapsed = 0f;
            var time = angle / _rotSpeed;
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
        
        public void Stop()
        {
            if(!_didSetup) return;
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            OnMovementStopped();
        }

        public void SyncCellToWorldPos()
        {
            CurrentCell = _map.GetCellPositionFromWorld(transform.position);
        }

        public async Task JumpToCell(Vector2Int cell, CancellationToken token, float time, float height)
        {
            TargetCell = cell;
            var tr = _unitView.transform;
            var p1 = tr.position;
            var p3 = _map.GetWorldFromCell(cell);
            var p2 = Vector3.Lerp(p1, p3, .5f) + Vector3.up * height;
            var elapsed = 0f;
            while (elapsed < time && !token.IsCancellationRequested)
            {
                var t = elapsed / time;
                var pos = Bezier.GetPosition(p1, p2, p3, t);
                tr.position = pos;
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            tr.position = p3;
            CurrentCell = cell;
        }
        
        
        
        private void Update()
        {
            if(!_didSetup || _map == null)
                return;
            foreach (var agent in _map.ActiveAgents)
            {
                if (agent == this)
                    continue;
                if (agent.CurrentCell == CurrentCell)
                {
                    CLog.LogRed($"{_unitView.gameObject.name} ERROR !! Another: {((HeroMovementManager)agent).gameObject.name}");
                    Debug.Break();
                    return;
                }
            }
        }
        
        private void OnDisable()
        {
            _didSetup = false;
            _isMoving = false;
            if (_map != null)
            {
                _map.ActiveAgents.Remove(this);
            }
        }
        
   
    }

}