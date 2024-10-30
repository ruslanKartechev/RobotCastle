using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellJudgementOfLight : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public SpellJudgementOfLight(HeroComponents components, SpellConfigJudgementOfLight config)
        {
            _config = config;
            _components = components;
            components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(components);
            _components.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        public float BaseSpellPower => _config.spellDamage[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
        public string name => "spell";
        public int order => 10;
        
        public float Decorate(float val) => val + BaseSpellPower;

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token = new CancellationTokenSource();
        
            Working(_token.Token);
        }
        
        private SpellConfigJudgementOfLight _config;
        private SpellParticlesByLevel _fxView;
        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private bool _isCasting;

        public void Stop()
        {
            if (_isCasting)
            {
                _components.animationEventReceiver.OnAttackEvent -= OnAnimationEvent;
                _isCasting = false;
            }
            _manaAdder.CanAdd = true;
            _isActive = false;
            _token?.Cancel();
        }

        private async void Working(CancellationToken token)
        {
            _isActive = true;
            _components.processes.Add(this);
            _manaAdder.CanAdd = false;
            
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var mask = _config.cellsMasksByTear[lvl];
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

            if (token.IsCancellationRequested) return;
            
            if (affectedEnemies is { Count: > 0 })
            {
                _isCasting = true;
                _components.animator.Play("Cast");
                _components.animationEventReceiver.OnAttackEvent += OnAnimationEvent;
                
                while(_isCasting && !token.IsCancellationRequested)
                    await Task.Yield();
                if (token.IsCancellationRequested) return;
                
                affectedEnemies = HeroesManager.GetHeroesInsideCellMask(mask, _components.transform.position, map, allEnemies);
                var fx = GetFxView();
                fx.transform.position = map.GetWorldFromCell(_components.state.currentCell);
                fx.Show(lvl);
                for (var i = affectedEnemies.Count - 1; i >= 0; i--)
                    _components.damageSource.DamageSpellAndPhys(affectedEnemies[i].Components.damageReceiver);
                _components.stats.ManaResetAfterFull.Reset(_components);
                _components.processes.Remove(this);
                _manaAdder.CanAdd = true;
                _isActive = false;
            }
        }

        private void OnAnimationEvent()
        {
            if (!_isActive) return;
            _components.animationEventReceiver.OnAttackEvent -= OnAnimationEvent;
            _isCasting = false;
            if (_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
        }

        private SpellParticlesByLevel GetFxView()
        {
            if (_fxView != null) return _fxView;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_JudgementOfLight);
            var instance = Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            _fxView = instance;
            return instance;
        }

     
    }
}