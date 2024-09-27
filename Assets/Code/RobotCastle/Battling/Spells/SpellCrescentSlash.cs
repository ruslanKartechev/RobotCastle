using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{

    
    public class SpellCrescentSlash : IFullManaListener, ISpellPowerGetter, IHeroProcess
    {
        public const int MaxDistance = 10;
        
        public float BaseSpellPower => _config.spellDamage[_view.stats.MergeTier];
        
        public float FullSpellPower
        {
            get
            {
                var v = BaseSpellPower * HeroesConfig.TierStatMultipliers[_view.stats.MergeTier];
                foreach (var dec in _view.stats.SpellPowerDecorators)
                    v = dec.Decorate(v);
                return v;
            }
        }
        
        public SpellCrescentSlash(HeroView view, SpellConfigCrescentSlash config)
        {
            _view = view;
            _config = config;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.ManaResetAfterFull = new ManaResetZero();
            _view.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(view);
        }

        private SpellConfigCrescentSlash _config;
        private HeroView _view;
        private CrescentSlashView _effect;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        private bool _isActive;

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _isActive = true;
            _manaAdder.CanAdd = false;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _token = new CancellationTokenSource();
            _view.processes.Add(this);
            CheckingPosition(_token.Token);
        }

        private void Launch()
        {
            _manaAdder.CanAdd = true;
            _isActive = false;
            _view.processes.Remove(this);
            _view.stats.ManaResetAfterFull.Reset(_view);
            if (_effect == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConfig.SpellFXPrefab_CrescentSlash);
                _effect = Object.Instantiate(prefab).GetComponent<CrescentSlashView>();
            }
            var lvl = (int)HeroesHelper.GetSpellTier(_view.stats.MergeTier);
            var hero = _view.GetComponent<IHeroController>();
            _effect.transform.SetPositionAndRotation(_view.transform.position, _view.transform.rotation);
            _effect.Launch(_config.cellsMasksByTear[lvl], _config.spellDamage[lvl], 
                _config.speed, hero.Battle.GetTeam(hero.TeamNum).enemyUnits);
        }

        public void Stop()
        {
            _isActive = false;
            _token?.Cancel();
        }

        private async void CheckingPosition(CancellationToken token)
        {
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            var map = _view.agent.Map;
            var lvl = (int)HeroesHelper.GetSpellTier(_view.stats.MergeTier);
            var mask = _config.cellsMasksByTear[lvl];
            var enemies = HeroesHelper.GetHeroesEnemies(_view);
            while (token.IsCancellationRequested == false)
            {
                var tr = _view.transform;
                var remainder = Mathf.Abs(tr.eulerAngles.y) % 90f;
                if (remainder < 2)
                {
                    var frw = tr.forward;
                    var cellCenter = _view.agent.CurrentCell;
                    var frwCellDir = new Vector2Int(Mathf.RoundToInt(frw.x), Mathf.RoundToInt(frw.z));
                    for (var stepInd = 0; stepInd < MaxDistance; stepInd++)
                    {
                        if (HeroesHelper.CheckIfAtLeastOneHeroInMask(mask, cellCenter, map, enemies))
                        {
                            Launch();
                            return;
                        }
                        cellCenter += frwCellDir;
                    }   
                }
                await Task.Yield();
                await Task.Yield();
            }
        }
        
    }
}