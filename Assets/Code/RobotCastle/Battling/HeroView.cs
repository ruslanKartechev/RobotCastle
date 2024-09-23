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
        
        public HeroStatsContainer Stats { get; set; }
        public HeroAnimationEventReceiver AnimationEventReceiver => _heroAnimationEvent;

        public IItemView MergeItemView => _mergeView;
        public IHeroAttackManager AttackManager { get; set; }
        public IHeroHealthManager HealthManager { get; set; }
        public IDamageReceiver DamageReceiver { get; set; }
        public HeroAttackInfoContainer AttackInfo { get; set; } = new ();
        
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

        public Transform projectileSpawnPoint => _projectileSpawnPoint;
        public ParticleSystem ShootParticles => _shootParticles;
    }
}