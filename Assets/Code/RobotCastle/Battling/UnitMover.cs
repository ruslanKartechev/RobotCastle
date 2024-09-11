using System.Threading.Tasks;
using Bomber;
using UnityEngine;
using SleepDev;

namespace RobotCastle.Battling
{
    public class UnitMover : MonoBehaviour, IPathfindingAgentAnimator
    {
        [SerializeField] private HeroView _unitView;
        private bool _didSetup;

        public HeroView UnitView
        {
            get => _unitView;
            set => _unitView = value;
        }
        
        public void OnMovementBegan()
        {
            _unitView.animator.Play("Run");
        }

        public void OnMovementStopped()
        {
            _unitView.animator.Play("Idle");
        }

        public async Task MoveToAttack(GameObject enemy)
        {
            if (!_didSetup)
            {
                CLog.LogRed("UnitMover NOT setup!!");
                return;
            }
        }
        
        public async Task MoveToCell(int x, int y)
        {
            if (!_didSetup)
            {
                CLog.LogRed("UnitMover NOT setup!!");
                return;
            }
            await _unitView.agent.MoveToCellAt(new Vector2Int(x, y));
        }

        public void Stop()
        {
            _unitView.agent.Stop();
        }
        
        public void SetupAgent()
        {
            _didSetup = true;
            _unitView.rb.isKinematic = false;
            _unitView.collider.isTrigger = false;
            _unitView.collider.enabled = true;
            _unitView.agent.RotationSpeed = 800;
            _unitView.agent.SpeedGetter = _unitView.Stats.MoveSpeed;
        }
    }
}