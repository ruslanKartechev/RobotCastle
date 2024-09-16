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

        public PathfindingAgent agent { get; set; }

        public HeroMovementManager unitMovementManager { get; set; }
        
        public HeroStatsContainer Stats { get; set; }

        public IItemView MergeItemView => _mergeView;
        
        [SerializeField] private BattleUnitUI _heroUI;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private UnitMergeView _mergeView;
    }
    
}