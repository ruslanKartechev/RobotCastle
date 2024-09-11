using Bomber;
using RobotCastle.UI;
using SleepDev.Ragdoll;
using UnityEngine;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroView : MonoBehaviour
    {
        public BattleUnitUI heroUI => _heroUI;
        public Animator animator => _animator;
        public Ragdoll ragdoll => _ragdoll;
        public Collider collider => _collider;
        public Rigidbody rb => _rb;

        public PathfindingAgent agent
        {
            get => _agent;
            set => _agent = value;
        }

        public UnitMover unitMover
        {
            get => _unitMover;
            set => _unitMover = value;
        }
        
        public HeroStatsContainer Stats { get; set; }
        
        [SerializeField] private BattleUnitUI _heroUI;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private PathfindingAgent _agent;
        [SerializeField] private UnitMover _unitMover;
    }
    
}