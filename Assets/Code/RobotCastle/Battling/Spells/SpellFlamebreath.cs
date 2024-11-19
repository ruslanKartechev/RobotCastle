using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellFlamebreath : Spell, IFullManaListener, IHeroProcess
    {
        public SpellFlamebreath(HeroComponents components, SpellConfigFlamebreath config)
        {
            _components = components;
            _config = config;
            Setup(config, out _manaAdder);
        }
        
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _isActive = true;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _components.processes.Add(this);
            Working(_token.Token);
        }
        
        
        public void Stop()
        {
            if (!_isActive) return;
            _isActive = false;
            _token?.Cancel();
            if (_isCasting)
            {
                _isCasting = false;
                _components.animationEventReceiver.OnAttackEvent += OnAttacked;
                _components.animator.SetBool("Cast", false);
            }
            _fx?.Hide();
        }
        
        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private SpellConfigFlamebreath _config;
        private SpellParticlesByLevel _fx;
        private bool _isCasting;

        private async void Working(CancellationToken token)
        {
            const int frameInside = 5;
            _manaAdder.CanAdd = false;
            
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var cone = new AttackRangeCone(_config.length[lvl]);
            var coneMask = new CellsMask(cone.GetCellsMask());
            var rotMask = new CellsMask(new List<Vector2Int>(10));
            var damage = _config.baseDamage + _components.stats.SpellPower.Get();

            var didFind = false;
            List<IHeroController> affectedEnemies = null;
            var map = _components.movement.Map;
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            
            while (!token.IsCancellationRequested && !didFind)
            {
                rotMask.SetAsRotated(coneMask, _components.transform.rotation);
                affectedEnemies = HeroesManager.GetHeroesInsideCellMask(rotMask, _components.transform.position, map, allEnemies);
                if (affectedEnemies.Count > 0)
                {
                    for (var i = 0; i < frameInside; i++)
                        await Task.Yield();
                    if (token.IsCancellationRequested) return;
                    affectedEnemies = HeroesManager.GetHeroesInsideCellMask(coneMask, _components.transform.position, map, allEnemies);
                    if(affectedEnemies.Count > 0)
                        didFind = true;
                }
                else
                {
                    await Task.Yield();
                }
            }

            if (token.IsCancellationRequested) return;
            var hero = _components.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            _isCasting = true;
            _components.animationEventReceiver.OnAttackEvent += OnAttacked;
            // CLog.LogBlue($"Calling animation: {Time.time}");
            _components.animator.SetBool("Cast", true);
            _components.animator.Play("Cast", 0, 0);
            var fx = GetFxView();
            fx.ShowUnTillOff(lvl, _components.SpellPoint);
            var t1 = Damaging(coneMask, rotMask, damage, token);
            var t2 = Rotating(token);
            await Task.WhenAny(t1, t2);
            if (token.IsCancellationRequested) return;
            _fx.gameObject.SetActive(false);
            _components.animator.SetBool("Cast", false);
            
            await HeroesManager.WaitGameTime(.5f, token);
            if (token.IsCancellationRequested || !_isActive) return;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
            _manaAdder.CanAdd = true;
            hero.ResumeCurrentBehaviour();
            _token?.Cancel();
            _isActive = false;
        }

        private async Task Damaging(CellsMask coneMask, CellsMask rotMask, float damage, CancellationToken token)
        {
            var map = _components.movement.Map;
            var args = new HeroDamageArgs(damage, EDamageType.Magical, _components);
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            for(var i = 0; i < _config.hitCount && !token.IsCancellationRequested; i++)
            {
                rotMask.SetAsRotated(coneMask, _components.transform.rotation);
                var affectedEnemies = 
                    HeroesManager.GetHeroesInsideCellMask(rotMask, _components.transform.position, map, allEnemies);
                foreach (var en in affectedEnemies)
                    _components.damageSource.Damage(en.Components.damageReceiver, args);
                await HeroesManager.WaitGameTime(_config.hitDelay, token);
            }
        }

        private async Task Rotating(CancellationToken token)
        {
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            if (allEnemies.Count == 0)
                return;
            var currentTargetValid = false;
            var pos = _components.transform.position;
            var minD2 = float.MaxValue;
            var target = allEnemies[0];
            var originalCell = target.Components.state.currentCell;
            while (!token.IsCancellationRequested)
            {
                foreach (var en in allEnemies)
                {
                    var d2 = (en.Components.transform.position - pos).sqrMagnitude;
                    if (d2 <= minD2)
                    {
                        minD2 = d2;
                        target = en;
                        currentTargetValid = true;
                    }
                }
                _components.movement.RotateIfNecessary(target.Components.transform, token);
                
                while (currentTargetValid && !token.IsCancellationRequested)
                {
                    if (target.IsDead)
                        break;
                    if (originalCell != target.Components.state.targetMoveCell)
                    {
                        currentTargetValid = false;
                    }
                    await Task.Yield();
                }
                                
                await Task.Yield();
            }
        }

        private void OnAttacked()
        {
            if (!_isActive) return;
            _isCasting = false;
            _components.animationEventReceiver.OnAttackEvent -= OnAttacked;
        }
        
        private SpellParticlesByLevel GetFxView()
        {
            if (_fx != null) return _fx;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_FlameBreath);
            var instance = UnityEngine.Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            _fx = instance;
            return instance;
        }

    }
}