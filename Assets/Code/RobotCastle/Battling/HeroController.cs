using Bomber;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroController : MonoBehaviour, IHeroController
    {
        [SerializeField] private HeroView _view;
        [SerializeField] private HeroSpellsContainer _spellsContainer;
        private HeroStatsContainer _stats;
        private IHeroBehaviour _currentBehaviour;
        private IUnitsItemsContainer _itemsContainer;
        private bool _didSetMap;
        
        public bool IsDead { get; private set; }
        public HeroView View => _view;
        
        public Battle Battle { get; set; }
        
        public int TeamNum { get; set; }
        
        private void AddHeroComponents()
        {
            _itemsContainer = gameObject.GetComponent<IUnitsItemsContainer>();
            _stats = gameObject.AddComponent<HeroStatsContainer>();
            var pathfinder = gameObject.AddComponent<Agent>();
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

            SetupStatsListeners();
        }
        
        
        public void InitHero(string id, int heroLevel, int mergeLevel)
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
            _view.Stats.ManaCurrent.SetBaseAndCurrent(0);
            _view.heroUI.AssignStatsTracking(_view);
        }

        /// <summary>
        /// Set Hero in no action state. All values are reset.
        /// </summary>
        public void SetIdle()
        {
            StopCurrentBehaviour();
            _view.movement.SetIdle();
        }

        public void MarkDead()
        {
            _view.movement.SetNullTargetCell();
            StopCurrentBehaviour();
            IsDead = true;
            Battle.OnKilled(this);
        }
        
        /// <summary>
        /// Use for returning to merge grid
        /// </summary>
        public void ResetForMerge()
        {
            IsDead = false;
            if (gameObject.TryGetComponent<IHeroMergeResetProcessor>(out var restart))
            {
                restart.ResetForMerge();
            }
            else
                CLog.LogRed($"[{gameObject.name}] IHeroRestartProcessor is null");
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
        
        private void SetupStatsListeners()
        {
            foreach (var mod in _view.SpellsContainer.modifiers)
            {
                mod.AddTo(gameObject);
            }
            if (_view.Stats.FullManaListener == null)
                _view.Stats.FullManaListener = new DefaultFullManaAction();
            if (_view.Stats.HealthReset == null)
                _view.Stats.HealthReset = new HealthResetFull();
            if (_view.Stats.ManaReset == null)
                _view.Stats.ManaReset = new ManaResetZero();
        }
    }
}