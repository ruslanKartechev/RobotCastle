using UnityEngine;

namespace Bomber
{
    public class PathfindingAgentAnimatorSimple : MonoBehaviour, IPathfindingAgentAnimator
    {
        private const string AnimKeyMove = "Walk";
        
        [SerializeField] private Animator _animator;

        public void OnMovementBegan()
        {
            _animator.SetBool(AnimKeyMove,true);
        }

        public void OnMovementStopped()
        {
            _animator.SetBool(AnimKeyMove,false);
        }
    }
}