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
        public HeroStatsManager stats { get; set; }
        public HeroAnimationEventReceiver animationEventReceiver => _heroAnimationEvent;
        public HeroItemsContainer heroItemsContainer => _heroItemsContainer;
        public IHeroAttackManager attackManager { get; set; }
        public IHeroHealthManager healthManager { get; set; }
        public IDamageReceiver damageReceiver { get; set; }
        public IKillProcessor killProcessor { get; set; }
        public HeroSpellsContainer spellsContainer { get; set;}
        public HeroStateData state { get; set; } = new();
        
        public Transform projectileSpawnPoint => _projectileSpawnPoint;
        public ParticleSystem shootParticles => _shootParticles;
        public IItemView mergeItemView => _mergeView;
        public HeroProcessesContainer processes { get; } = new();
        public float StunnedFxHeight => _stunnedFxHeight;
        
        [SerializeField] private float _stunnedFxHeight = 1.5f;
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