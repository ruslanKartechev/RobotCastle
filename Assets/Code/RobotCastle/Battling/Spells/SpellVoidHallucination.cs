using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellVoidHallucination : Spell, IFullManaListener, IHeroProcess
    {
        public const string GhostId = "saras_hallucination";
        
        public SpellVoidHallucination(SpellConfigVoidHallucination config, HeroComponents components)
        {
            _config = config;
            _components = components;
            Setup(config, out _manaAdder);
        }
    
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _components.processes.Add(this);
            _isActive = true;
            Working(_token.Token);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _manaAdder.CanAdd = true;
                Delete();
            }
            _token?.Cancel();
            _token = new CancellationTokenSource();
        }

        private CancellationTokenSource _token;
        private SpellConfigVoidHallucination _config;
        private ConditionedManaAdder _manaAdder;
        private List<IHeroController> _spawnedHeroes = new (5);
        private float _damage;
        private IHeroController _hero;

        private async void Working(CancellationToken token)
        {
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            _manaAdder.CanAdd = false;
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, _config.duration);
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            _hero = _components.gameObject.GetComponent<IHeroController>();
            _damage = _config.damageAdded[lvl] + _config.damageSpMultiplier * _components.stats.SpellPower.Get();
            _spawnedHeroes.Clear();
            BattleManager.SpawnHeroesInBattle(GhostId, _config.ghostsCount, _hero, _components.state.currentCell,
                _spawnedHeroes, InitHero);
            
            await HeroesManager.WaitGameTime(_config.duration, token);
            if (token.IsCancellationRequested) return;
                
            Delete();
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
            _manaAdder.CanAdd = true;
            _isActive = false;
        }

        private void Delete()
        {
            var bm = ServiceLocator.Get<BattleManager>();
            foreach (var h in _spawnedHeroes)
            {
                if (h == null || h.Components == null) continue;
                _components.summonedContainer.Remove(h);
                h.StopCurrentBehaviour();
                bm.battle.RemovePlayer(h);
                UnityEngine.Object.Destroy(h.Components.gameObject);
            }
            _spawnedHeroes.Clear();
        }

        private void InitHero(IHeroController hero)
        {
            hero.InitHero(_components.stats.HeroId, _components.stats.HeroLvl, 
                _components.stats.MergeTier, new List<ModifierProvider>());
            hero.Components.StatCollectionId = _components.StatCollectionId;
            hero.Components.heroUI.ManaUI.Hide();
            hero.Components.stats.ManaResetAfterFull.Reset(_hero.Components);
            hero.Components.movement.InitAgent(_components.movement.Map);
            hero.Battle = _hero.Battle;
            hero.TeamNum = _hero.TeamNum;
            hero.Components.stats.Attack.SetBaseAndCurrent(_damage); 
            hero.Components.heroUI.Level.SetLevel(_hero.Components.stats.MergeTier);
            hero.SetBehaviour(new HeroAttackEnemyBehaviour());
            var bm = ServiceLocator.Get<BattleManager>();
            var statCollector = bm.playerStatCollector;
            statCollector.AddHero(hero);
            _components.summonedContainer.Add(hero);
            // bm.battle.playersAlive.Add(hero);
        }
    }
    
    
}