using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Utils;
using SleepDev;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RobotCastle.Battling
{
    public class SpellSoulLink : Spell, IFullManaListener, IHeroProcess
    {
        public SpellSoulLink(SpellConfigSoulLink config, HeroComponents components)
        {
            _config = config;
            _components = components;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _hero = _components.gameObject.GetComponent<IHeroController>();
        }
     
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            try
            {
                var enemies = _hero.Battle.enemiesAlive;
                if (enemies.Count < 2)
                {
                    _components.stats.ManaResetAfterFull.Reset(_components);
                    _isActive = false;
                }
                else
                {
                    _isActive = true;
                    _manaAdder.CanAdd = false;
                    _damageData.Reset();
                    _damageData.onBroken = OnMaxDamage;
                    _components.processes.Add(this);
                    _components.animationEventReceiver.OnAttackEvent += OnAnimEvent;
                    _hero.PauseCurrentBehaviour();
                    _components.animator.Play("Cast",0,0);
                }
            }
            catch (System.Exception ex)
            {
                CLog.LogRed($"Exception: {ex.Message}. Trace: {ex.StackTrace}");
            }
        }
        
        public void Stop()
        {
            if(_isActive)
            {
                _damageData.Reset();
                UnbindCurrent();
                Complete();
            }
        }

        private CancellationTokenSource _token;
        private SpellConfigSoulLink _config;
        private ConditionedManaAdder _manaAdder;
        private SoulLinkFX _fx;
        private DamageData _damageData = new DamageData();
        private IHeroController _hero;
        private readonly List<IHeroController> _boundEnemies = new (3);
        private readonly List<DamageShareMod> _modifiers = new (3);
        private int _damageOnBroken;


        private void OnAnimEvent()
        {
            if (!_isActive) return;
            if (_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
            
            _hero.ResumeCurrentBehaviour();
            var enemies = _hero.Battle.enemiesAlive;
            _components.animationEventReceiver.OnAttackEvent -= OnAnimEvent;
            if (enemies.Count < 2)
            {
                _isActive = false;
                _components.stats.ManaResetAfterFull.Reset(_components);
                _components.processes.Remove(this);
                return;
            }
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            _damageData.maxDamage = _config.maxDamage[lvl];
            _damageOnBroken = _config.damageOnBroken[lvl];
            Bind(enemies);
            _token = new CancellationTokenSource();
            Work(_token.Token);
        }
        
        private SoulLinkFX GetFxView()
        {
            if(_fx == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_SoulLink);
                _fx = Object.Instantiate(prefab).GetComponent<SoulLinkFX>();
            }
            _fx.transform.position = _components.transform.position;
            _fx.transform.rotation = _components.transform.rotation;
            return _fx;
        }
        
        private void Bind(List<IHeroController> enemies)
        {
            UnbindCurrent();
            for (var i = 0; i < enemies.Count && i < 3; i++)
            {
                _boundEnemies.Add(enemies[i]);
            }
            var fx = GetFxView();
            foreach (var en in _boundEnemies)
            {
                var mod = new DamageShareMod(en, _components, _boundEnemies, _damageData, fx);
                _modifiers.Add(mod);
            }
            fx.Show(_boundEnemies);
        }

        private void UnbindCurrent()
        {
            if (_modifiers.Count > 0)
            {
                foreach (var en in _modifiers)
                    en.UnBind();
            }
            _boundEnemies.Clear();
            _modifiers.Clear();
        }

        private void OnMaxDamage()
        {
            CLog.LogGreen($"Max Damage");
            var args = new HeroDamageArgs(_damageOnBroken, EDamageType.Magical, _components, DamageShareMod.DamageId);
            for (var i = _boundEnemies.Count - 1; i >= 0; i--)
            {
                var h = _boundEnemies[i];
                if (h.IsDead)
                    continue;
                _components.damageSource.Damage(h.Components.damageReceiver, args);
            }

            UnbindCurrent();
            Complete();
            _components.processes.Remove(this);
        }

        private void Complete()
        {
            _isActive = false;
            _fx.HideAll();
            _manaAdder.CanAdd = true;
            _token?.Cancel();
        }

        private async void Work(CancellationToken token)
        {
            await Task.Delay(_config.duration.SecToMs(), token);
            if (!_isActive) return;
            UnbindCurrent();
            Complete();
            _components.processes.Remove(this);
        }
        

        private class DamageData
        {
            public int maxDamage;
            public Action onBroken;

            public void Reset()
            {
                _didCall = false;
                _totalDamage = 0;
            }
            
            public int totalDamage
            {
                get => _totalDamage;
                set
                {
                    _totalDamage = value;
                    if (!_didCall && _totalDamage >= maxDamage)
                    {
                        _didCall = true;
                        onBroken?.Invoke();
                    }
                }
            }

            private int _totalDamage;
            private bool _didCall;
        }

        private class DamageShareMod : IDamageTakenModifiers, IKIllModifier
        {
            public const string DamageId = "soul_link";
            public int priority => 100;
            
            public int order => 200;
            
            public IHeroController enemy;
            public HeroComponents myHero;
            public DamageData damage;
            public SoulLinkFX fx;
            public List<IHeroController> boundEnemies;

            public DamageShareMod(IHeroController enemy, 
            HeroComponents myHero, 
            List<IHeroController> enemies,
            DamageData damageData,
            SoulLinkFX fx)
            {
                this.damage = damageData;
                this.boundEnemies = enemies;
                this.enemy = enemy;
                this.myHero = myHero;
                this.fx = fx;
                enemy.Components.healthManager.AddModifier(this);
                enemy.Components.killProcessor.AddModifier(this);
            }

            public void UnBind()
            {
                enemy.Components.healthManager.RemoveModifier(this);
            }
                    
            public HeroDamageArgs Apply(HeroDamageArgs damageInput)
            {
                if (damageInput.srsId != DamageId)
                {
                    // CLog.LogYellow($"Sharing damage: {damageInput.amount}");
                    var args = new HeroDamageArgs(damageInput.amount, damageInput.type, myHero, DamageId);
                    for (var i = boundEnemies.Count - 1; i >= 0; i--)
                    {
                        var h = boundEnemies[i];
                        if (h != enemy)
                            myHero.damageSource.Damage(h.Components.damageReceiver, args);
                    }
                    damage.totalDamage += (int)args.amount;
                }
                return damageInput;
            }

            public void OnKilled(HeroComponents components)
            {
                var h = components.GetComponent<IHeroController>();
                boundEnemies.Remove(h);
                fx.Remove(h);
            }
        }

  
    }
}