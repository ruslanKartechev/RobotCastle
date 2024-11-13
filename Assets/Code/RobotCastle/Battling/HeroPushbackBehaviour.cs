using System;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroPushbackBehaviour : IHeroBehaviour
    {
        public HeroPushbackBehaviour(int distance, float time)
        {
            _distance = distance;
            _duration = time;
        }
        
        public string BehaviourID => "pushback";

        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            _hero = hero;
            _callback = endCallback;
            _token = new CancellationTokenSource();
            _particles = ServiceLocator.Get<ISimplePoolsManager>().GetOne(ObjectPoolConstants.FxPoolId_Stunned);
            Pushing(_token.Token);
        }

        public void Stop()
        {
            _token.Cancel();
        }

        private const float JumpHeight = 1f;
        private Action<IHeroBehaviour> _callback;
        private IHeroController _hero;
        private IPoolItem _particles;
        private float _duration;
        private CancellationTokenSource _token;
        private int _distance;

        private async void Pushing(CancellationToken token)
        {
            _hero.Components.attackManager.Stop();
            _hero.Components.animator.Play("Stunned", 0,0);
            
            var map = _hero.Components.movement.Map;
            var movement = _hero.Components.movement;
            movement.Stop();

            var tr = _hero.Components.transform;
            var angle = tr.eulerAngles.y;
            var backDirection = Vector2Int.down;
            var turns = Mathf.RoundToInt(angle);
            switch (turns)
            {
                case 0: backDirection = Vector2Int.down; break; // 0
                case 1: backDirection = Vector2Int.left; break; // 90
                case 2: backDirection = Vector2Int.up; break; // 180
                case 3: backDirection = Vector2Int.right; break; // 270
                case 4: backDirection = Vector2Int.down; break; // 360
            }
            Vector2Int endCell = default;
            var didFindCell = false;
            var currentCell = movement.CurrentCell;
            for (var d = _distance; d > 0; d--)
            {
                var cell = currentCell + backDirection * d;
                if (map.IsOutOfBounce(cell))
                    continue;
                if (map.IsFullyFree(cell))
                {
                    didFindCell = true;
                    endCell = cell;
                    break;
                }
            }
            if (!didFindCell)
            {
                CLog.LogRed($"Didn't find cell");
                endCell = currentCell;
            }
            var elapsed = 0f;
            
            var pt = _particles.GetGameObject().transform;
            var originalParent = pt.parent;
            pt.parent = movement.transform;
            pt.localPosition = Vector3.up * _hero.Components.StunnedFxHeight;
            
            _particles.PoolShow();
            movement.TargetCell = endCell;

            var p1 = movement.transform.position;
            var p3 = map.GetWorldFromCell(endCell);
            var p2 = Vector3.Lerp(p1, p3, .5f) + Vector3.up * JumpHeight;
            // Debug.DrawLine(p3, p3 + Vector3.up * 5f, Color.yellow, 5f);
            
            var r2 = Quaternion.LookRotation(p1 - p3);
            var r1 = movement.transform.rotation;
            while (elapsed < _duration && !token.IsCancellationRequested)
            {
                var t = elapsed / _duration;
                var p = Bezier.GetPosition(p1,p2,p3, t);
                var r = Quaternion.Lerp(r1, r2, t);
                tr.position = p;
                tr.rotation = r;
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            tr.SetPositionAndRotation(p3, r2);
            movement.CurrentCell = endCell;

            await Task.Delay(200, token);
            if (token.IsCancellationRequested) return;
            
            _particles.PoolHide();
            pt.transform.parent = originalParent;
            
            _callback?.Invoke(this);

        }
    }
}