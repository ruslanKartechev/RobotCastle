using Bomber;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private HeroView _view;
        [SerializeField] private HeroSpellsContainer _spellsContainer;
        private HeroStatsContainer _stats;
        private IHeroBehaviour _currentBehaviour;
        private IUnitsItemsContainer _itemsContainer;
        private bool _didSetMap;
        
        public bool IsDead { get; private set; }
        public HeroView HeroView => _view;
        
        public Battle Battle { get; set; }
        
        public int TeamNum { get; set; }
        
        private void AddHeroComponents()
        {
            _itemsContainer = gameObject.GetComponent<IUnitsItemsContainer>();
            _stats = gameObject.AddComponent<HeroStatsContainer>();
            var pathfinder = gameObject.AddComponent<PathfindingAgent>();
            var unitMover = gameObject.AddComponent<HeroMovementManager>();
            unitMover.UnitView = _view;
            pathfinder.rb = _view.rb;
            pathfinder.movable = transform;
            pathfinder.PathfindingAgentAnimatorGO = gameObject;
            _view.agent = pathfinder;
            _view.movement = unitMover;
            _view.Stats = _stats;

            var damage = gameObject.GetComponent<IHeroIDamageReceiver>();
            var health = gameObject.GetComponent<IHeroHealthManager>();
            var attack = gameObject.GetComponent<IHeroAttackManager>();
            attack.Hero = this;
            _view.DamageReceiver = damage;
            _view.HealthManager = health;
            _view.AttackManager = attack;
        }

        public void InitComponents(string id, int heroLevel, int mergeLevel)
        {
            AddHeroComponents();
            _stats.Init(id, heroLevel, mergeLevel);
        }

        public void UpdateMap(bool force = false)
        {
            if (_didSetMap && !force)
                return;
            _didSetMap = true;
            _view.agent.InitAgent(ServiceLocator.Get<IMap>());
        }

        public void PrepareForBattle()
        {
            _view.movement.SetupAgent();
            _view.heroUI.HealthUI.Set(_view.Stats.HealthCurrent, _view.Stats.ManaMax);
            _view.heroUI.ManaUI.Set(_view.Stats.ManaCurrent, _view.Stats.ManaMax);
        }

        public void SetIdle()
        {
            StopCurrentBehaviour();
            _view.movement.SetIdle();
            // _view.animator.Play("Idle", 0, 0);
        }

        public void MarkDead()
        {
            StopCurrentBehaviour();
            IsDead = true;
            Battle.RemoveDead(this);
        }

        public void StopCurrentBehaviour()
        {
            if (_currentBehaviour != null)
                _currentBehaviour.Stop();
            _currentBehaviour = null;
        }
        
        public void SetBehaviour(IHeroBehaviour behaviour)
        {
            if (_currentBehaviour != null)
                _currentBehaviour.Stop();
            _currentBehaviour = behaviour;
            _currentBehaviour.Activate(this, OnBehaviourEnd);
        }

        private void OnBehaviourEnd(IHeroBehaviour behaviour)
        {
        }

        private void OnDisable()
        {
            _currentBehaviour?.Stop();
        }
    }
}