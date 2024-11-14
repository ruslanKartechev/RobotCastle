using System;
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroPulledBehaviour : IHeroBehaviour
    {
        public HeroPulledBehaviour(float time, Vector2Int targetCell)
        {
            this._pullTime = time;
            this._targetCell = targetCell;
        }
        
        public string BehaviourID => "pulled";
        
        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            CLog.Log($"[{nameof(HeroPulledBehaviour)}] Activated on: {hero.Components.gameObject.name}");
            _hero = hero;
            _callback = endCallback;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            Working(_token.Token);
        }

        public void Stop()
        {
            _token?.Cancel();
            _hero.Components.animator.Play("Idle", 0, 0);
        }

        private float _pullTime;
        private Vector2Int _targetCell;
        private CancellationTokenSource _token;
        private IHeroController _hero;
        private Action<IHeroBehaviour> _callback;

        private async void Working(CancellationToken token)
        {
            var comp = _hero.Components;
            comp.movement.Stop();
            comp.attackManager.Stop();
            comp.animator.SetBool("Pulled", true);
            comp.animator.Play("Pulled", 0, 0);
            comp.state.SetPulled(true);
            comp.state.targetMoveCell = _targetCell;
            var tr = comp.transform;
            var p1 = tr.position;
            var p2 = comp.movement.Map.GetWorldFromCell(_targetCell);
            var r1 = tr.rotation;
            var r2 = Quaternion.LookRotation(p2 - p1);
            var elapsed = 0f;
            Debug.DrawLine(p1, p1 + Vector3.up, Color.red, 10f);
            Debug.DrawLine(p2, p2 + Vector3.up, Color.red, 10f);
            
            while (!token.IsCancellationRequested && elapsed < _pullTime)
            {
                var t = elapsed / _pullTime;
                var p = Vector3.Lerp(p1, p2, t);
                var r = Quaternion.Lerp(r1, r2, t);
                tr.SetPositionAndRotation(p, r);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }

            if (token.IsCancellationRequested) return;
            
            tr.SetPositionAndRotation(p2, r2);
            comp.animator.SetBool("Pulled", false);
            comp.animator.Play("Idle", 0, 0);
            comp.state.SetPulled(false);
            comp.movement.SyncCellToWorldPos();            

            _callback?.Invoke(this);
        }
        
    }
}