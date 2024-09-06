using RobotCastle.UI;
using SleepDev;
using SleepDev.Ragdoll;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitView : MonoBehaviour
    {
        public BattleUnitUI UnitUI => _unitUI;
        public Animator animator => _animator;
        public Ragdoll ragdoll => _ragdoll;
        public Collider collider1 => _collider;
        
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider _collider;
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private BattleUnitUI _unitUI;
    }
}