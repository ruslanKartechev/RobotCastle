using System.Collections.Generic;
using Bomber;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroController : MonoBehaviour, IHeroController
    {
        public bool IsDead { get; set; }
        public HeroView View => _view;
        
        public Battle Battle { get; set; }
        
        public int TeamNum { get; set; }
        
        [SerializeField] private HeroView _view;
        private HeroStatsManager _stats;
        private IHeroBehaviour _currentBehaviour;
        private bool _didSetMap;


        public void InitHero(string id, int heroLevel, int mergeLevel, List<ModifierProvider> spells)
        {
            AddHeroComponents();
            _stats.LoadAndSetHeroStats(id, heroLevel, mergeLevel);
            _view.SpellsContainer.AddModifierProviders(spells);
            _view.SpellsContainer.ApplyAllModifiers(_view);
            SetupStatsListeners();
        }

        public void UpdateMap(bool force = false)
        {
            if (_didSetMap && !force)
                return;
            _didSetMap = true;
            _view.agent.InitAgent(ServiceLocator.Get<IMap>());
        }


        public void MarkDead()
        {
            _view.movement.SetNullTargetCell();
            StopCurrentBehaviour();
            IsDead = true;
            Battle.OnKilled(this);
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
            if (_view.Stats.FullManaListener == null)
                _view.Stats.FullManaListener = new DefaultFullManaAction();
            if (_view.Stats.HealthReset == null)
                _view.Stats.HealthReset = new HealthResetFull();
            if (_view.Stats.ManaReset == null)
                _view.Stats.ManaReset = new ManaResetZero();
        }
        
        private void AddHeroComponents()
        {
            _stats = gameObject.AddComponent<HeroStatsManager>();
            var health = new HeroHealthManager(_view);
            var pathfinder = gameObject.AddComponent<Agent>();
            var unitMover = gameObject.AddComponent<HeroMovementManager>();
            unitMover.UnitView = _view;
            pathfinder.rb = _view.rb;
            pathfinder.movable = transform;
            pathfinder.PathfindingAgentAnimatorGO = gameObject;
            _view.agent = pathfinder;
            _view.movement = unitMover;
            _view.Stats = _stats;
            _view.KillProcessor = new HeroDeathProcessor(_view);
            _view.SpellsContainer = new HeroSpellsContainer();

            var attack = gameObject.GetComponent<IHeroAttackManager>();
            attack.Hero = this;
            _view.DamageReceiver = health;
            _view.HealthManager = health;
            _view.AttackManager = attack;

        }
    }
}