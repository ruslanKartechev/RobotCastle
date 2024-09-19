using System.Collections.Generic;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class EnemiesManager : MonoBehaviour
    {
        [SerializeField] private bool _initOnStart = true;
        [SerializeField] private EnemiesFactory _enemiesFactory;
        [SerializeField] private GridView _gridView;
        private bool _didInit;

        public List<HeroController> Enemies => _enemiesFactory.SpawnedEnemies;

        private void Start()
        {
            if(_initOnStart)
                Init();
        }

        public void Init()
        {
            if (_didInit)
            {
                CLog.LogRed($"[{nameof(EnemiesManager)}] Already did init");
                return;
            }
            _didInit = true;
            _gridView.BuildGridFromView();
            _enemiesFactory.GridView = _gridView;
        }

        public void SpawnPreset(string preset)
        {
            _enemiesFactory.SpawnPreset(preset);
        }

        public void SpawnNewEnemy(SpawnMergeItemArgs args)
        {
            _enemiesFactory.SpawnNew(args);
        }
    }
}