using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellOrderElimination : Spell, IFullManaListener, IHeroProcess
    {
        public const string GhostId = "rie_doll";
        
        public SpellOrderElimination(SpellConfigOrderElimination config, HeroComponents components)
        {
            _config = config;
            _components = components;
            Setup(config, out _manaAdder);
            _modifierNormalDamage = new DamageModifierChangeTypeAndAmount()
            {
                amount = (int)_config.normalDamage,
                type = EDamageType.Magical
            };
            _modifierNormalDamage.order = 100;
            _modifierStartDamage = new DamageModifierChangeTypeAndAmount()
            {
                amount = (int)_config.initialDamage,
                type = EDamageType.Magical
            };
            _modifierStartDamage.order = 90;
        }
    
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            
            var countAlive = 0;
            var bm = ServiceLocator.Get<BattleManager>();
            for (var i = _spawnedHeroes.Count-1; i >= 0 ; i--)
            {
                if (_spawnedHeroes[i] == null || _spawnedHeroes[i].Components == null)
                {
                    _spawnedHeroes.RemoveAt(i);
                    continue;
                }
                var h = _spawnedHeroes[i];
                if (!h.IsDead)
                {
                    countAlive++;
                }
                else
                {
                    // _spawnedHeroes.RemoveAt(i);
                    // h.StopCurrentBehaviour();
                    // bm.battle.RemovePlayer(h);
                    // UnityEngine.Object.Destroy(h.Components.gameObject);
                }
            }
            
            if (countAlive >= _config.maxCount)
            {
                CLog.Log($"[SpellOrderElimination] Already max amount of dolls");
                _isActive = false;
                _components.stats.ManaResetAfterFull.Reset(_components);
                return;
            }
            _components.processes.Add(this);
            _isActive = true;
            Working(_token.Token);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _manaAdder.CanAdd = false;
                foreach (var mm in _numberedModifiers)
                    mm.Disable();
                _numberedModifiers.Clear();
                Delete();
            }
            _token?.Cancel();
            _token = new CancellationTokenSource();
        }

        private CancellationTokenSource _token;
        private SpellConfigOrderElimination _config;
        private ConditionedManaAdder _manaAdder;
        private List<IHeroController> _spawnedHeroes = new (5);
        private List<NumberedDamageModifier> _numberedModifiers = new (5);
        private DamageModifierChangeTypeAndAmount _modifierStartDamage;
        private DamageModifierChangeTypeAndAmount _modifierNormalDamage;

        private float _damage;
        private IHeroController _hero;
        

        private async void Working(CancellationToken token)
        {
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            _hero = _components.gameObject.GetComponent<IHeroController>();
            _damage = _config.normalDamage;
            CLog.Log("Spawning.........");
            BattleManager.SpawnHeroesInBattle(GhostId, 1, _hero, _components.state.currentCell,
                _spawnedHeroes, InitHero);
            
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, .3f);
            _components.processes.Remove(this);
            _isActive = false;
        }

        private void Delete()
        {
            var bm = ServiceLocator.Get<BattleManager>();
            foreach (var h in _spawnedHeroes)
            {
                if (h == null)
                    continue;
                h.StopCurrentBehaviour();
                bm.battle.RemovePlayer(h);
                UnityEngine.Object.Destroy(h.Components.gameObject);
            }
            _spawnedHeroes.Clear();
        }

        private void InitHero(IHeroController hero)
        {
            var comps = hero.Components;
            hero.InitHero(GhostId, _components.stats.HeroLvl, 
                _components.stats.MergeTier, new List<ModifierProvider>());
            comps.StatCollectionId = _components.StatCollectionId;
            comps.heroUI.ManaUI.Hide();
            comps.stats.ManaResetAfterFull.Reset(_hero.Components);
            comps.movement.InitAgent(_components.movement.Map);
            hero.Battle = _hero.Battle;
            hero.TeamNum = _hero.TeamNum;
            comps.stats.Attack.SetBaseAndCurrent(_damage); 
            comps.heroUI.Level.SetLevel(_hero.Components.stats.MergeTier);
            
            _components.summonedContainer.Add(hero);
            var bm = ServiceLocator.Get<BattleManager>();
            bm.battle.AddPlayer(hero);
            var statCollector = bm.playerStatCollector;
            statCollector.AddHero(hero);
            comps.damageSource.AddModifier(_modifierNormalDamage);
            var mod = new NumberedDamageModifier(_modifierStartDamage, hero, 1);
            mod.Add();
            _numberedModifiers.Add(mod);
            
            
            BattleManager.PrepareForBattle(new List<IHeroController>(){hero});
            hero.SetBehaviour(new HeroAttackEnemyBehaviour());
            // bm.battle.playersAlive.Add(hero);
        }
    }
}