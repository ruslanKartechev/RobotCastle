using System.Collections.Generic;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellSummonDead : Spell, IFullManaListener, IHeroProcess
    {
        public string name => "summon_dead";

        public int order => 1;
        
        public SpellSummonDead(HeroComponents components, SpellConfigSummonDead config)
        {
            _components = components;
            _config = config;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = new ConditionedManaAdder(_components);
        }
        
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _isActive = true;
            _components.stats.ManaResetAfterFull.Reset(_components);
            var args = new List<SpawnMergeItemArgs>(_config.enemiesToSpawn.Count);
            foreach (var core in _config.enemiesToSpawn)
            {
                var arg = new SpawnMergeItemArgs(core);
                arg.coreData.level = _components.stats.MergeTier;
                arg.usePreferredCoordinate = false;
                args.Add(arg);
            }
            _components.agent.SetCurrentCellFromWorldPosition();
            BattleManager.SetClosestAvailableDesiredPositions(args, _components.state.currentCell);
            var bm = ServiceLocator.Get<BattleManager>();
            try
            {
                bm.AddNewEnemiesDuringBattle(args);
            }
            catch (System.Exception ex)
            {
                CLog.LogRed($"========= Exception: {ex.Message}\n{ex.StackTrace}");
            }
            _isActive = false;
        }

        public float Decorate(float val) => val;

        public void Stop()
        {
            _isActive = false;
        }

        private SpellConfigSummonDead _config;
    }
}