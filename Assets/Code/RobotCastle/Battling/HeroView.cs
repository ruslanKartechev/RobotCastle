using Bomber;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev.Ragdoll;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroView : MonoBehaviour
    {
        public BattleUnitUI heroUI => _heroUI;
        public Animator animator => _animator;
        public Ragdoll ragdoll => _ragdoll;
        public Collider collider => _collider;
        public Rigidbody rb => _rb;
        public Agent agent { get; set; }
        public HeroMovementManager movement { get; set; }
        public HeroStatsManager Stats { get; set; }
        public HeroAnimationEventReceiver AnimationEventReceiver => _heroAnimationEvent;
        public HeroItemsContainer HeroItemsContainer => _heroItemsContainer;
        public IHeroAttackManager AttackManager { get; set; }
        public IHeroHealthManager HealthManager { get; set; }
        public IDamageReceiver DamageReceiver { get; set; }
        public IKillProcessor KillProcessor { get; set; }
        public AttackTargetData AttackData { get; set; } = new ();
        public HeroSpellsContainer SpellsContainer { get; set;}
        public HeroDamageSource DamageSource { get; set; }
        public Transform projectileSpawnPoint => _projectileSpawnPoint;
        public ParticleSystem ShootParticles => _shootParticles;
        public IItemView MergeItemView => _mergeView;
        
        
        [SerializeField] private HeroItemsContainer _heroItemsContainer;
        [SerializeField] private BattleUnitUI _heroUI;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private MergeView_Hero _mergeView;
        [SerializeField] private HeroAnimationEventReceiver _heroAnimationEvent;
        [Space(10)]
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private ParticleSystem _shootParticles;


    }
}