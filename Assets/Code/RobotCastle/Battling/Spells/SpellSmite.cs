using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellSmite : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        
        public SpellSmite(SpellConfigSmite config, HeroComponents components)
        {
            _components = components;
            _config = config;
            components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(components);
            _components.stats.SpellPower.AddPermanentDecorator(this);
        }
    
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _isActive = true;
            _manaAdder.CanAdd = false;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            Working(_token.Token);
        }

        public string name => "smite";
        
        public int order => 1;
        
        public float Decorate(float val)
        {
            return BaseSpellPower + val;
        }

        public void Stop()
        {
            if (_isCasting)
            {
                _isCasting = false;
                _components.animationEventReceiver.OnAttackEvent -= OnAttack;
            }
            if(_isActive)
                _token?.Cancel();
            _isActive = false;
        }
        
        public float BaseSpellPower => _config.damageMag[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];

        private SpellConfigSmite _config;
        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private SpellParticlesByLevel _fxView;
        private bool _isCasting;


        private async void Working(CancellationToken token)
        {
            _components.processes.Add(this);
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var mask = new CellsMask()            {
                mask = new (){new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(1,1)}
            };
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            var map = _components.agent.Map;
            
            const int waitMs = (int)(.25f * 1000);
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            const int frameInside = 5;
            List<IHeroController> affectedEnemies = null;
            var didFind = false;
            while (!token.IsCancellationRequested && !didFind)
            {
                affectedEnemies = HeroesManager.GetHeroesInsideCellMask(mask, _components.transform.position, map, allEnemies);
                if (affectedEnemies.Count > 0)
                {
                    for (var i = 0; i < frameInside; i++)
                        await Task.Yield();
                    if (token.IsCancellationRequested) return;
                    affectedEnemies = HeroesManager.GetHeroesInsideCellMask(mask, _components.transform.position, map, allEnemies);
                    if(affectedEnemies.Count > 0)
                        didFind = true;
                }
                else
                    await Task.Delay(waitMs, token);
            }
            if (affectedEnemies is { Count: > 0 })
            {
                _isCasting = true;
                _components.animator.Play("Attack");
                _components.animationEventReceiver.OnAttackEvent += OnAttack;
                
                while(_isCasting && !token.IsCancellationRequested)
                    await Task.Yield();
                if (token.IsCancellationRequested) return;
                
                affectedEnemies = HeroesManager.GetHeroesInsideCellMask(mask, _components.transform.position, map, allEnemies);
                var fx = GetFxView();
                fx.Show(lvl);
                for (var i = affectedEnemies.Count - 1; i >= 0; i--)
                {
                    affectedEnemies[i].SetBehaviour(new HeroStunnedBehaviour(_config.stunDuration));
                    _components.damageSource.DamageSpellAndPhys(_config.damagePhys[lvl], 
                        _config.damageMag[lvl], affectedEnemies[i].Components.damageReceiver);
                }
            }
            _isCasting = false;
            _isActive = false;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
            _manaAdder.CanAdd = true;
        }
        
        private void OnAttack()
        {
            _components.animationEventReceiver.OnAttackEvent -= OnAttack;
            _isCasting = false;
        }

        private SpellParticlesByLevel GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Smite);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }
    }
    
}