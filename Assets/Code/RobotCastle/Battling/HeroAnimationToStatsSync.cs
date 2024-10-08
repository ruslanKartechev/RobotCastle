using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroAnimationToStatsSync
    {
        private HeroView _view;
        private bool _inited;
        
        public HeroAnimationToStatsSync(HeroView view)
        {
            _view = view;
        }

        public void Init(bool sync)
        {
            if (_inited) return;
            _inited = true;
            var ms = _view.stats.MoveSpeed;
            ms.OnValueChange += UpdateMoveSpeed;
            ms.OnDecoratorAdded += UpdateMoveSpeed;
            ms.OnDecoratorRemoved += UpdateMoveSpeed;
            
            var atks = _view.stats.AttackSpeed;
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
            var ms = _view.stats.MoveSpeed; 
            ms.OnValueChange -= UpdateMoveSpeed;
            ms.OnDecoratorAdded -= UpdateMoveSpeed;
            ms.OnDecoratorRemoved -= UpdateMoveSpeed;

            var atks = _view.stats.AttackSpeed; 
            atks.OnValueChange -= UpdateAttackSpeed;
            atks.OnDecoratorAdded -= UpdateAttackSpeed;
            atks.OnDecoratorRemoved -= UpdateAttackSpeed;
        }

        public void ForceUpdateMoveSpeed()
        {
            UpdateMoveSpeed(_view.stats.MoveSpeed);
        }

        public void ForceUpdateAttackSpeed()
        {
            UpdateAttackSpeed(_view.stats.AttackSpeed);
        }
        
        private void UpdateMoveSpeed(Stat stat)
        {
            var val = stat.Get();
            var animSpeed = val * HeroesConstants.SpeedStatToAnimationFactor / HeroesConstants.SpeedStatFactor;
            // CLog.Log($"Updating move animation speed. Stat: {val}. Speed: {animSpeed}");
            _view.animator.SetFloat("MoveSpeed", animSpeed);
        }
     
        private void UpdateAttackSpeed(Stat stat)
        {
            var val = stat.Get();
            CLog.Log($"[{_view.gameObject.name}] Updating attack animation speed. Stat: {val}");
            _view.animator.SetFloat("Speed", val);
        }
    }
}