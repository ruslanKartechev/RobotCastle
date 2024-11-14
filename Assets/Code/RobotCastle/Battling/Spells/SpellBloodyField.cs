using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellBloodyField : Spell, IFullManaListener, IHeroProcess
    {
        public SpellBloodyField(SpellConfigBloodyField config, HeroComponents components)
        {
            _components = components;
            _config = config;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
        }
    
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            CLog.Log($"[{nameof(SpellBloodyField)}] Starting");
            _isActive = true;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            Working(_token.Token);
        }

        public void Stop()
        {
            if (!_isActive) return;
            _token?.Cancel();            
            _isActive = false;
            _manaAdder.CanAdd = true;
            _components.state.SetOutOfMap(false);
        }

        private ConditionedManaAdder _manaAdder;
        private SpellConfigBloodyField _config;
        private SpellParticlesByLevel _fx;
        private CancellationTokenSource _token;

        private async void Working(CancellationToken token)
        {
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            _manaAdder.CanAdd = false;
            var heroes = HeroesManager.GetHeroesEnemies(_components);
            var myPos = _components.movement.CurrentCell;
            heroes.Sort((a, b) =>
            {
                var d1 = (a.Components.state.currentCell - myPos).sqrMagnitude;
                var d2 = (b.Components.state.currentCell - myPos).sqrMagnitude;
                return d1.CompareTo(d2);
            });
            var map = _components.movement.Map;
            
            IHeroController targetEnemy = null;
            Vector2Int targetCell = default;
            foreach (var h in heroes)
            {
                var p = h.Components.state.currentCell;
                var (didFind, cell) = HeroesManager.GetClosestFreeCell(p, map);
                if (didFind)
                {
                    targetEnemy = h;
                    targetCell = cell;
                    break;
                }
            }
            if (targetEnemy == null)
            {
                CLog.LogRed($"[SpellBloodyField] Cannot find an enemy");
                _isActive = false;
                _manaAdder.CanAdd = true;
                _components.stats.ManaResetAfterBattle.Reset(_components);
                return;
            }
            _components.state.TargetCell = targetCell;
            _components.state.SetOutOfMap(true);
            _components.animator.Play("Burrow", 0, 0);
            var downY = 7f;
            var tr = _components.transform;
            var p1 = tr.position;
            var p2 = p1 - new Vector3(0, downY, 0);
            var elapsed = 0f;
            var time = _config.jumpTime;
            while (!token.IsCancellationRequested && elapsed < time)
            {
                var p = Vector3.Lerp(p1, p2, elapsed / time);
                tr.position = p;
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if(token.IsCancellationRequested) return;
            
            var newCellPosWorld = map.GetWorldFromCell(targetCell); 
            p1 = tr.position = p2;
            p2 = newCellPosWorld;
            p2.y = p1.y;
            elapsed = 0f;
            time = _config.hideTime;
            while (!token.IsCancellationRequested && elapsed < time)
            {
                var p = Vector3.Lerp(p1, p2, elapsed / time);
                tr.position = p;
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if(token.IsCancellationRequested) return;
            
            p1 = tr.position;
            p2 = newCellPosWorld;
            time = _config.jumpTime2;
            elapsed = 0f;
            _components.state.currentCell = _components.state.TargetCell = targetCell;
            
            _components.animator.Play("Unburrow", 0, 0);
            while (!token.IsCancellationRequested && elapsed < time)
            {
                var p = Vector3.Lerp(p1, p2, elapsed / time);
                tr.position = p;
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if(token.IsCancellationRequested) return;
            _components.state.SetOutOfMap(false);
            
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            var inRange = HeroesManager.GetHeroesInsideCellMask(_config.mask, p2, map, allEnemies);
            foreach (var hh in inRange)
                hh.SetBehaviour(new HeroStunnedBehaviour(_config.stunTime));
            
            hero.ResumeCurrentBehaviour();
            _isActive = false;
            _manaAdder.CanAdd = true;
            _components.stats.ManaResetAfterFull.Reset(_components);
            
        }
        
    }
}