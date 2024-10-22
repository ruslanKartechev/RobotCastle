using System.Collections.Generic;
using Bomber;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroController : MonoBehaviour, IHeroController
    {
        public bool IsDead { get; set; }
        
        public Battle Battle { get; set; }
        
        public int TeamNum { get; set; }
        public string GUID => _components.GUID;
        public HeroComponents Components => _components;
   
        public void InitHero(string id, int heroLevel, int mergeLevel, List<ModifierProvider> spells)
        {
            _components.GUID = System.Guid.NewGuid().ToString();
            CLog.LogGreen($"GUID: {GUID}");
            AddHeroComponents();
            _stats.LoadAndSetHeroStats(id, heroLevel, mergeLevel);
            _components.statAnimationSync.Init(true);
            _components.spellsContainer.AddModifierProviders(spells);
            _components.spellsContainer.ApplyAllModifiers(_components);
            SetStatsComponentsIfMissing();
        }

        public void UpdateMap(bool force = false)
        {
            if (_didSetMap && !force)
                return;
            _didSetMap = true;
            _components.agent.InitAgent(ServiceLocator.Get<IMap>());
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
        
             
        [SerializeField] private HeroComponents _components;
        private HeroStatsManager _stats;
        private IHeroBehaviour _currentBehaviour;
        private bool _didSetMap;


        private void OnBehaviourEnd(IHeroBehaviour behaviour)
        {
            if (IsDead) return;
            _currentBehaviour = new HeroAttackEnemyBehaviour();
            _currentBehaviour.Activate(this, OnBehaviourEnd);
        }

        private void OnDisable()
        {
            _components.statAnimationSync?.Stop();
            _currentBehaviour?.Stop();
        }
        
        private void SetStatsComponentsIfMissing()
        {
            var stats = _components.stats; 
            if (stats.FullManaListener == null)
                stats.FullManaListener = new DefaultFullManaAction();
            if (stats.HealthReset == null)
                stats.HealthReset = new HealthResetFull();
            if (stats.ManaResetAfterBattle == null)
                stats.ManaResetAfterBattle = new ManaResetZero();
            if (stats.ManaAdder == null)
                stats.ManaAdder = new SimpleManaAdder(_components);
        }
        
        private void AddHeroComponents()
        {
            _stats = gameObject.AddComponent<HeroStatsManager>();
            var health = new HeroHealthManager(_components);
            var pathfinder = gameObject.AddComponent<Agent>();
            var unitMover = gameObject.AddComponent<HeroMovementManager>();
            unitMover.UnitView = _components;
            pathfinder.rb = _components.rb;
            pathfinder.movable = transform;
            pathfinder.PathfindingAgentAnimatorGO = gameObject;
            _components.agent = pathfinder;
            _components.movement = unitMover;
            _components.stats = _stats;
            _components.killProcessor = new HeroDeathProcessor(_components);
            _components.spellsContainer = new HeroSpellsContainer();
            _components.damageSource = new HeroDamageSource(_components);
            _components.statAnimationSync = new HeroAnimationToStatsSync(_components);

            var attack = gameObject.GetComponent<IHeroAttackManager>();
            attack.Hero = this;
            _components.damageReceiver = health;
            _components.healthManager = health;
            _components.attackManager = attack;

        }
    }
}