using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RobotCastle.Battling
{
    public class SpellCheapshot : Spell, IFullManaListener, IHeroProcess
    {
        public SpellCheapshot(SpellConfigCheapShot config, HeroComponents components)
        {
            _config = config;
            _components = components;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
        }
    
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _isActive = true;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _manaAdder.CanAdd = false;
            Working(_token.Token);
        }

        public void Stop()
        {
            if (!_isActive) return;
            _isActive = false;
            _token?.Cancel();
            _manaAdder.CanAdd = true;
            _fxView?.Hide();
        }

        private SpellConfigCheapShot _config;
        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private CheapshotView _fxView;
        private bool _isWaiting;
        
        private async void Working(CancellationToken token)
        {
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            var cellFree = false;
            Vector2Int targetCell = default;
            var mask = new CellsMask(new List<Vector2Int>() { Vector2Int.up });
            var rotMask = new CellsMask(mask.mask);
            do
            {
                rotMask.SetAsRotated(mask, _components.transform.rotation);
                targetCell = _components.state.currentCell + rotMask.mask[0];
                // CLog.LogWhite($"Target cell {targetCell}. My Cell {_components.state.currentCell}");
                if (_components.movement.Map.IsFullyFree(targetCell))
                {
                    cellFree = true;
                    break;
                }
                await Task.Yield();
            } while (!cellFree && !token.IsCancellationRequested);
            if (token.IsCancellationRequested) return;

            var enemies = HeroesManager.GetHeroesEnemies(_components);
            IHeroController enemy = null;
            var maxD2 = 0;
            var mPos = _components.state.currentCell;
            foreach (var en in enemies)
            {
                if (en.IsDead || en.Components.state.isPulled)
                    continue;
                var d2 = (mPos - en.Components.state.currentCell).sqrMagnitude;
                if (d2 >= maxD2)
                {
                    maxD2 = d2;
                    enemy = en;
                }
            }
            if (enemy == null)
            {
                CLog.LogRed($"[SpellCheapshot] Enemy is null");
                Complete();
                return;
            }
            enemy.StopCurrentBehaviour();
            enemy.Components.state.SetPulled(true);
            _isWaiting = true;
            var fx = GetFxView();
            fx.Pull(_components.SpellPoint, enemy, _config.grabTime, OnGrabbed);
            while (!token.IsCancellationRequested && _isWaiting)
                await Task.Yield();
            if (token.IsCancellationRequested) return;

            var b = new HeroPulledBehaviour(_config.pullTime, targetCell);
            enemy.SetBehaviour(b);
            await HeroesManager.WaitGameTime(_config.pullTime, token);

            if (token.IsCancellationRequested) return;
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            enemy.SetBehaviour(new HeroStunnedBehaviour(_config.stunTime));
            
            _components.stats.ManaResetAfterFull.Reset(_components);
            _fxView.Hide();
            Complete();
            hero.ResumeCurrentBehaviour();
        }

        private void OnGrabbed()
        {
            _isWaiting = false;
        }

        private void Complete()
        {
            _isActive = false;
            _manaAdder.CanAdd = false;
            _token?.Cancel();
        }
        
        private CheapshotView GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Cheapshot);
                _fxView = Object.Instantiate(prefab).GetComponent<CheapshotView>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }
    }
    
}