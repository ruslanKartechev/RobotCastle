using System.Threading;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellSoulBurn : Spell, IFullManaListener, IHeroProcess
    {
        public SpellSoulBurn(SpellConfigSoulBurn config, HeroComponents components)
        {
            _config = config;
            _components = components;
            Setup(config, out _manaAdder);
            var mask = new CellsMask(new AttackRangeRectangle(_config.width, _config.height).GetCellsMask());
            _hitAction = new HitActionAllInRange()
            {
                components = components,
                mask = mask
            };
        }

        private SpellConfigSoulBurn _config;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        private IAttackHitAction _prevHitAction;
        private HitActionAllInRange _hitAction;
        private SpellParticlesOnHero _fxView;

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _isActive = true;
            _token?.Cancel();
            _token = new CancellationTokenSource();
  
            _components.processes.Add(this);
            Working(_token.Token);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                if(_fxView != null)
                    _fxView.gameObject.SetActive(false);
                _token?.Cancel();
                if (_components.attackManager.HitAction == _hitAction)
                    _components.attackManager.HitAction = _prevHitAction;
            }
        }

        private async void Working(CancellationToken token)
        {
            _manaAdder.CanAdd = false;
            _prevHitAction = _components.attackManager.HitAction;
            _components.attackManager.HitAction = _hitAction;
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, _config.duration);
            var fx = GetFxView();
            fx.ShowUntilOff(_components.transform);
            await HeroesManager.WaitGameTime(_config.duration, token);
            if (token.IsCancellationRequested) return;
            fx.gameObject.SetActive(false);
            _components.processes.Remove(this);
            if (_components.attackManager.HitAction == _hitAction)
                _components.attackManager.HitAction = _prevHitAction;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _manaAdder.CanAdd = true;
            _isActive = false;
        }
        
        private SpellParticlesOnHero GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_SoulBurn);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesOnHero>();
            }
            return _fxView;
        }

    }
}