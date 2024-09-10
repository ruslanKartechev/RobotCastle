using Bomber;
using RobotCastle.UI;
using SleepDev.Ragdoll;
using UnityEngine;
using SleepDev;

namespace RobotCastle.Battling
{
    public class UnitView : MonoBehaviour
    {
        public BattleUnitUI UnitUI => _unitUI;
        public Animator animator => _animator;
        public Ragdoll ragdoll => _ragdoll;
        public Collider collider => _collider;
        public Rigidbody rb => _rb;
        public PathfindingAgent agent => _agent;
        public UnitMover unitMover => _unitMover;
        
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private BattleUnitUI _unitUI;
        [SerializeField] private PathfindingAgent _agent;
        [SerializeField] private UnitMover _unitMover;
    }
    
}