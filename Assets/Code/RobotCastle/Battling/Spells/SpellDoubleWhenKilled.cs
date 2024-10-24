using System.Collections.Generic;
using RobotCastle.Core;

namespace RobotCastle.Battling
{
    public class SpellDoubleWhenKilled : IKIllModifier
    {
        public SpellDoubleWhenKilled(List<SpawnMergeItemArgs> spawnArgs, HeroComponents components)
        {
            _components = components;
            _components.killProcessor.AddModifier(this);
            var count = spawnArgs.Count;
            _spawnArgs = new List<SpawnMergeItemArgs>(count);
            for (var i = 0; i < count; i++)
                _spawnArgs.Add(new SpawnMergeItemArgs(spawnArgs[i]));
        }

        public int order => 100;
        
        public void OnKilled(HeroComponents components)
        {
            _components.killProcessor.RemoveModifier(this);
            _components.agent.SetCurrentCellFromWorldPosition();
            BattleManager.SetClosestAvailableDesiredPositions(_spawnArgs, _components.agent.CurrentCell);
            foreach (var arg in _spawnArgs)
            {
                arg.coreData.level = components.stats.MergeTier;
                arg.usePreferredCoordinate = false;
            }
            ServiceLocator.Get<BattleManager>().AddNewEnemiesDuringBattle(_spawnArgs);
        }
        
        private List<SpawnMergeItemArgs> _spawnArgs;
        private HeroComponents _components;
        
    }
}