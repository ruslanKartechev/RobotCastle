using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellEarthquake : Spell, IFullManaListener, IHeroProcess
    { 
        public SpellEarthquake(SpellConfigEarthquake config, HeroComponents components)
        {
            _config = config;
            _components = components;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
        }

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token = new CancellationTokenSource();
            Working(_token.Token);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _manaAdder.CanAdd = true;
                _isCasting = false;
                _components.animationEventReceiver.OnAttackEvent -= OnCast;
                _token?.Cancel();
            }
        }
        
        private SpellConfigEarthquake _config;
        private CancellationTokenSource _token;
        private SpellParticlesByLevel _fxView;
        private ConditionedManaAdder _manaAdder;
        private bool _isCasting;

        private async void Working(CancellationToken token)
        {
            _isActive = true;
            _manaAdder.CanAdd = false;
            _components.processes.Add(this);
            _isCasting = true;
            var atkRange = new AttackRangeRectangle(_config.squareRange, _config.squareRange);
            var mask = new CellsMask(atkRange.GetCellsMask());
            
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            _components.animator.Play("Cast", 0, 0);
            _components.animationEventReceiver.OnAttackEvent += OnCast;
            while(_isCasting && !token.IsCancellationRequested)
                await Task.Yield();
            if (token.IsCancellationRequested) return;

            var map = _components.movement.Map;
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            var enemies = BattleManager.GetBestTargetForAttack(hero, null);
            var maskCenter = _components.state.currentCell;
            if (enemies is { Count: > 0 })
                maskCenter = enemies[0].Components.state.currentCell;
            var worldCenter = map.GetWorldFromCell(maskCenter);
            var affectedEnemies = HeroesManager.GetHeroesInsideCellMask(mask, maskCenter, map, allEnemies);
            foreach (var en in affectedEnemies)
            {
                _components.damageSource.DamageSpell(en.Components.damageReceiver);
            }
            var fx = GetFxView();
            fx.PlayLevelAtPoint(worldCenter, 0);
            hero.ResumeCurrentBehaviour();
            _isActive = false;
            _manaAdder.CanAdd = true;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
        }

        private void OnCast()
        {
            if (!_isActive) return;
            _components.animationEventReceiver.OnAttackEvent -= OnCast;
            _isCasting = false;
            if (_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
        }
    
        
        private SpellParticlesByLevel GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Earthquake);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }


    }
}