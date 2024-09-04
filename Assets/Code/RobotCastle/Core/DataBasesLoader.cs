using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Core
{
    public class DataBasesLoader : MonoBehaviour, IGameLoader
    {
        [SerializeField] private GlobalConfig _globalConfig;
        [SerializeField] private HeroesDataBase _heroesDataBase;
        [SerializeField] private LevelsDataBase _levelsDataBase;
        [SerializeField] private MergeGridViewDataBase _mergeGridView;
        [SerializeField] private ViewDataBaseContainer _unitsDataBase;
        
        public void Load()
        {
            ServiceLocator.Bind<GlobalConfig>(_globalConfig);
            ServiceLocator.Bind<HeroesDataBase>(_heroesDataBase);
            ServiceLocator.Bind<LevelsDataBase>(_levelsDataBase);
            ServiceLocator.Bind<MergeGridViewDataBase>(_mergeGridView);
            ServiceLocator.Bind<ViewDataBaseContainer>(_unitsDataBase);
            
            _unitsDataBase.Load();
            ServiceLocator.Bind<ViewDataBase>(_unitsDataBase.DataBase);
        }
    }
}