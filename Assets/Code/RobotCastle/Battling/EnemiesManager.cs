using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class EnemiesManager : MonoBehaviour
    {
        public List<IHeroController> AllEnemies => _enemiesFactory.SpawnedEnemies;
        public EnemiesFactory factory => _enemiesFactory;
        
        public void Init()
        {
            if (_didInit)
            {
                CLog.LogRed($"[{nameof(EnemiesManager)}] Already did init");
                return;
            }
            _didInit = true;
            ServiceLocator.Get<GridViewsContainer>().AddGridView(_gridView);
            _gridView.BuildGridFromView();
            _enemiesFactory.GridView = _gridView;
        }

        public IHeroController SpawnNewEnemy(SpawnArgs args, int heroLvl = 0, bool addToList = true)
        {
            var h = _enemiesFactory.SpawnNew(args, heroLvl);
            if(addToList)
                AllEnemies.Add(h);
            return h;
        }

        public void DestroyCurrentUnits()
        {
            var barsPanel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            foreach (var en in AllEnemies)
            {
                var merge = en.Components.gameObject.GetComponent<IItemView>();
                MergeFunctions.ClearCell(_gridView.BuiltGrid, merge.itemData);
                if (en.Components.heroUI.gameObject.TryGetComponent<HeroUIWrapper>(out var wrapper))
                    barsPanel.ReturnToPool(wrapper);
            }
            for (var i = AllEnemies.Count - 1; i >= 0; i--)
            {
                Destroy(AllEnemies[i].Components.gameObject);
            }
            AllEnemies.Clear();
        }

        public void IncreaseEnemyForcesBy(float percent)
        {
            var enemies = AllEnemies;
            var totalCount = enemies.Count;
            var additionalCount = Mathf.RoundToInt(totalCount * percent);
            if(additionalCount == 0 && percent > 0)
                additionalCount = 1;
            CLog.Log($"[{nameof(EnemiesManager)}] Adding: {additionalCount} more enemies ({Mathf.RoundToInt(percent*100)}%)");
            var countLeft = additionalCount;
            var newEnemies = new List<IHeroController>(additionalCount);
            var allFreeCells = new List<ICellView>(50);
            foreach (var cellView in _gridView.Grid)
            {
                if(cellView.itemView == null)
                    allFreeCells.Add(cellView);
            }
            var pool = ServiceLocator.Get<ISimplePoolsManager>();
            const string particlesId = "new_enemy_spawn";
            while (countLeft > 0)
            {
                for (var i = 0; i < enemies.Count && countLeft > 0; i++)
                {
                    var original = enemies[i];
                    var cell = allFreeCells.RemoveRandom();
                    var mergeView = original.Components.gameObject.GetComponent<IItemView>();
                    var args = new SpawnArgs(mergeView.itemData.core);
                    args.usePreferredCoordinate = true;
                    args.preferredCoordinated = cell.cell.Coord;
                    var hero = _enemiesFactory.SpawnNew(args);
                    newEnemies.Add(hero);
                    countLeft--;
                    var particles = pool.GetOne(particlesId) as OneTimeParticles;
                    particles.Show(cell.WorldPosition);
                    if (allFreeCells.Count == 0)
                    {
                        CLog.LogRed("[IncreaseEnemyForcesBy] No more free cells!");
                        return;
                    }
                }
            }
            AllEnemies.AddRange(newEnemies);
        }

        
        public void RaiseEnemiesTierAll(int additionalVal)
        {
            RaiseEnemiesTier(AllEnemies, additionalVal);
        }        
        
        public void RaiseEnemiesTier(List<IHeroController> enemies, int additionalVal)
        {
            var pool = ServiceLocator.Get<ISimplePoolsManager>();
            const string particlesId = "new_enemy_spawn";
            foreach (var hero in enemies)
            {
                var itemView = hero.Components.gameObject.GetComponent<IItemView>();
                var lvl = itemView.itemData.core.level;
                if (lvl < MergeConstants.HeroMaxMergeLvl)
                {
                    lvl += additionalVal;
                    if (lvl > MergeConstants.HeroMaxMergeLvl)
                        lvl = MergeConstants.HeroMaxMergeLvl;
                    itemView.itemData.core.level = lvl;
                    itemView.UpdateViewToData();
                    
                    var particles = pool.GetOne(particlesId) as OneTimeParticles;
                    particles.Show(itemView.Transform.position);
                }
            }    
        }
        
        [SerializeField] private bool _initOnStart = true;
        [SerializeField] private EnemiesFactory _enemiesFactory;
        [SerializeField] private GridView _gridView;
        private bool _didInit;
        
        private void Start()
        {
            if(_initOnStart)
                Init();
        }

    }
}