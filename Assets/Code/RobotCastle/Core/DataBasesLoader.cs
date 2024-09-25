using RobotCastle.Battling;
using RobotCastle.Data;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Core
{
    public class DataBasesLoader : MonoBehaviour, IGameLoader
    {
        [SerializeField] private GlobalConfig _globalConfig;
        [SerializeField] private MergeGridViewDataBase _mergeGridView;
        [SerializeField] private LevelsDataBase _levelsDb;
        [SerializeField] private ViewDataBaseContainer _viewDb;
        [SerializeField] private HeroesDatabaseContainer _heroesDb;
        [SerializeField] private ModifiersDataBase _modifiersDataBase;
        
        public void Load()
        {
            ServiceLocator.Bind<GlobalConfig>(_globalConfig);
            ServiceLocator.Bind<LevelsDataBase>(_levelsDb);
            ServiceLocator.Bind<MergeGridViewDataBase>(_mergeGridView);
            ServiceLocator.Bind<HeroesDatabaseContainer>(_heroesDb);
            ServiceLocator.Bind<ViewDataBaseContainer>(_viewDb);
            ServiceLocator.Bind<ModifiersDataBase>(_modifiersDataBase);
            _modifiersDataBase.Init();
            _viewDb.Load();
            _heroesDb.Load();
            ServiceLocator.Bind<ViewDataBase>(_viewDb.viewDb);
            ServiceLocator.Bind<DescriptionsDataBase>(_viewDb.descriptionsDb);
            ServiceLocator.Bind<HeroesDatabase>(_heroesDb.DataBase);
        }
    }
}