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
            _view.statAnimationSync.Init(true);
            _view.spellsContainer.AddModifierProviders(spells);
            _view.spellsContainer.ApplyAllModifiers(_view);
            SetStatsComponentsIfMissing();
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
            if (IsDead)
                return;
            if (_currentBehaviour != null)
                _currentBehaviour.Stop();
            _currentBehaviour = behaviour;
            _currentBehaviour.Activate(this, OnBehaviourEnd);
        }

        private void OnBehaviourEnd(IHeroBehaviour behaviour)
        {
            if (IsDead) return;
            _currentBehaviour = new HeroAttackEnemyBehaviour();
            _currentBehaviour.Activate(this, OnBehaviourEnd);
        }

        private void OnDisable()
        {
            _view.statAnimationSync.Stop();
            _currentBehaviour?.Stop();
        }
        
        private void SetStatsComponentsIfMissing()
        {
            var stats = _view.stats; 
            if (stats.FullManaListener == null)
                stats.FullManaListener = new DefaultFullManaAction();
            if (stats.HealthReset == null)
                stats.HealthReset = new HealthResetFull();
            if (stats.ManaResetAfterBattle == null)
                stats.ManaResetAfterBattle = new ManaResetZero();
            if (stats.ManaAdder == null)
                stats.ManaAdder = new SimpleManaAdder(_view);
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
            _view.stats = _stats;
            _view.killProcessor = new HeroDeathProcessor(_view);
            _view.spellsContainer = new HeroSpellsContainer();
            _view.stats.DamageCalculator = new DefaultDamageCalculator(_view.stats);
            _view.statAnimationSync = new HeroAnimationToStatsSync(_view);

            var attack = gameObject.GetComponent<IHeroAttackManager>();
            attack.Hero = this;
            _view.damageReceiver = health;
            _view.healthManager = health;
            _view.attackManager = attack;

        }
    }
}