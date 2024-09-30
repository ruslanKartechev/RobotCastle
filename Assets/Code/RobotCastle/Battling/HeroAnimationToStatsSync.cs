using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroAnimationToStatsSync
    {
        private HeroView _unitView;
        private bool _inited;
        
        public HeroAnimationToStatsSync(HeroView view)
        {
            _unitView = view;
        }

        public void Init(bool sync)
        {
            if (_inited) return;
            _inited = true;
            var ms = _unitView.stats.MoveSpeed;
            ms.OnValueChange += UpdateMoveSpeed;
            ms.OnDecoratorAdded += UpdateMoveSpeed;
            ms.OnDecoratorRemoved += UpdateMoveSpeed;
            
            var atks = _unitView.stats.AttackSpeed;
            atks.OnValueChange += UpdateAttackSpeed;
            atks.OnDecoratorAdded += UpdateAttackSpeed;
            atks.OnDecoratorRemoved += UpdateAttackSpeed;
            if (sync)
            {
                UpdateMoveSpeed(ms);
                UpdateAttackSpeed(atks);
            }
        }

        public void Stop()
        {
            if (!_inited) return;
            _inited = false;
            var ms = _unitView.stats.MoveSpeed; 
            ms.OnValueChange -= UpdateMoveSpeed;
            ms.OnDecoratorAdded -= UpdateMoveSpeed;
            ms.OnDecoratorRemoved -= UpdateMoveSpeed;

            var atks = _unitView.stats.AttackSpeed; 
            atks.OnValueChange -= UpdateAttackSpeed;
            atks.OnDecoratorAdded -= UpdateAttackSpeed;
            atks.OnDecoratorRemoved -= UpdateAttackSpeed;
        }

        public void ForceUpdateMoveSpeed()
        {
            UpdateMoveSpeed(_unitView.stats.MoveSpeed);
        }

        public void ForceUpdateAttackSpeed()
        {
            UpdateAttackSpeed(_unitView.stats.AttackSpeed);
        }
        
        private void UpdateMoveSpeed(Stat stat)
        {
            var val = stat.Get();
            var animSpeed = val * HeroesConstants.SpeedStatToAnimationFactor / HeroesConstants.SpeedStatFactor;
            CLog.LogGreen($"Updating move animation speed. Stat: {val}. Speed: {animSpeed}");
            _unitView.animator.SetFloat("MoveSpeed", animSpeed);
        }
     
        private void UpdateAttackSpeed(Stat stat)
        {
            var val = stat.Get();
            CLog.LogYellow($"Updating attack animation speed. Stat: {val}");
            _unitView.animator.SetFloat("Speed", val);
        }
    }
}