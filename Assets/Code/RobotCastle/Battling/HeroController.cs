using Bomber;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private HeroView _unitView;
        [SerializeField] private HeroSpellsContainer _spellsContainer;
        private HeroStatsContainer _stats;
        private IUnitBehaviour _currentBehaviour;
        private IUnitsItemsContainer _itemsContainer;
        private bool _didSetMap;

        public HeroView HeroView => _unitView;
        
        private void AddHeroComponents()
        {
            _itemsContainer = gameObject.GetComponent<IUnitsItemsContainer>();
            _stats = gameObject.AddComponent<HeroStatsContainer>();
            var pathfinder = gameObject.AddComponent<PathfindingAgent>();
            var unitMover = gameObject.AddComponent<HeroMovementManager>();
            unitMover.UnitView = _unitView;
            pathfinder.rb = _unitView.rb;
            pathfinder.movable = transform;
            pathfinder.PathfindingAgentAnimatorGo = gameObject;
            _unitView.agent = pathfinder;
            _unitView.unitMovementManager = unitMover;
            _unitView.Stats = _stats;
        }

        public void InitAsPlayerHero(string id, int heroLevel, int mergeLevel)
        {
            AddHeroComponents();
            _stats.Init(id, heroLevel, mergeLevel);
        }

        public void InitAsEnemyHero(string id, int heroLevel, int mergeLevel)
        {
            AddHeroComponents();
            _stats.Init(id, heroLevel, mergeLevel);
        }

        public void UpdateMap(bool force = false)
        {
            if (_didSetMap && !force)
                return;
            _didSetMap = true;
            _unitView.agent.InitAgent(ServiceLocator.Get<IMap>());
        }

        public void PrepareForBattle()
        {
            _unitView.unitMovementManager.SetupAgent();
        }
    }
}