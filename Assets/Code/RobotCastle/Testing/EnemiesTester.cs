using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class EnemiesTester : MonoBehaviour
    {
        [SerializeField] private bool _doSpawnHero = true;
        [SerializeField] private SpawnMergeItemArgs _heroSpawnArgs;
        [SerializeField] private bool _addItems;
        [SerializeField] private List<CoreItemData> _heroItems;
        [Space(10)]
        [SerializeField] private bool _doSpawnEnemy = false;
        [SerializeField] private SpawnMergeItemArgs _enemySpawnArgs;


        [ContextMenu("Run")]
        public void Run()
        {
            if (_doSpawnHero)
            {
                var spawner = ServiceLocator.Get<IHeroesAndUnitsFactory>();
                if (_addItems)
                    spawner.SpawnHeroWithItems(_heroSpawnArgs, _heroItems, out var view);
                else
                    spawner.SpawnHeroOrItem(_heroSpawnArgs, out var view);
            }

            if (_doSpawnEnemy)
            {
                var enemiesManager = FindObjectOfType<EnemiesManager>();
                enemiesManager.SpawnNewEnemy(_enemySpawnArgs);
            }
            
            var battle = FindObjectOfType<BattleManager>();
            battle.BeginBattle();

        }

        public void GetOneEnemyAndUnit()
        {
            
        }

        public void GetAllUnits()
        {
            
        }

    }
}