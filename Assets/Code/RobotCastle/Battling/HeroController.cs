using Bomber;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private HeroView _unitView;
        private HeroStatsContainer _stats;
        private IUnitBehaviour _currentBehaviour;
        private IUnitsItemsContainer _itemsContainer;
        private bool _didInit;
        
        private void AddHeroComponents()
        {
            _itemsContainer = gameObject.GetComponent<IUnitsItemsContainer>();
            _stats = gameObject.AddComponent<HeroStatsContainer>();
            var pathfinder = gameObject.AddComponent<PathfindingAgent>();
            var unitMover = gameObject.AddComponent<UnitMover>();
            unitMover.UnitView = _unitView;
            pathfinder.rb = _unitView.rb;
            pathfinder.movable = transform;
            pathfinder.iPathfindingAgentAnimatorGo = gameObject;
            _unitView.agent = pathfinder;
            _unitView.unitMover = unitMover;
            _unitView.Stats = _stats;
        }

        public void InitAsPlayerHero(string id, int heroLevel, int mergeLevel)
        {
            AddHeroComponents();
            _stats.Init(id, heroLevel, mergeLevel);
        }
        
        public void UpdateMap(bool force = false)
        {
            if (_didInit && !force)
                return;
            _didInit = true;
            _unitView.agent.InitAgent(ServiceLocator.Get<IMap>());
        }

        public void SetHeroLevel(string id, int charLvl, int mergeTier)
        {
            _stats.Init(id, charLvl, mergeTier);
        }

        public void PrepareForBattle()
        {
            _unitView.unitMover.SetupAgent();
        }
    }
}