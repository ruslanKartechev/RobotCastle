using System.Threading.Tasks;
using Bomber;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitMover : MonoBehaviour, IPathfindingAgentAnimator
    {
        [SerializeField] private UnitView _unitView;
        
        public void OnMovementBegan()
        {
            _unitView.animator.Play("Run");
        }

        public void OnMovementStopped()
        {
            _unitView.animator.Play("Idle");
        }
        
        public async Task MoveToCell(int x, int y)
        {
            _unitView.rb.isKinematic = false;
            _unitView.collider.isTrigger = false;
            _unitView.collider.enabled = true;
            CLog.LogBlue($"MOVING TO cell {x},{y}");            
            await _unitView.agent.MoveToCellAt(new Vector2Int(x, y));
        }

        public void Stop()
        {
            _unitView.agent.Stop();
        }
    }
}