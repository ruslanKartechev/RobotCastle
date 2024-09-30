using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellCrescentSlash : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public const int MaxDistance = 10;
        
        public float BaseSpellPower => _config.spellDamage[(int)HeroesHelper.GetSpellTier(_view.stats.MergeTier)];
        public string name => "spell";
        public int priority => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellCrescentSlash(HeroView view, SpellConfigCrescentSlash config)
        {
            _view = view;
            _config = config;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(view);
            _view.stats.SpellPower.AddPermanentDecorator(this);
        }

        private SpellConfigCrescentSlash _config;
        private CrescentSlashView _fxView;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            CLog.Log($"[{_view.gameObject.name}] [{nameof(SpellCrescentSlash)}]");
            _isActive = true;
            _manaAdder.CanAdd = false;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _token = new CancellationTokenSource();
            _view.processes.Add(this);
            CheckingPosition(_token.Token);
        }
        
        public void Stop()
        {
            _isActive = false;
            _token?.Cancel();
        }


        private void Launch()
        {
            _manaAdder.CanAdd = true;
            _isActive = false;
            _view.processes.Remove(this);
            _view.stats.ManaResetAfterFull.Reset(_view);
            var fx = GetFxView();
            var lvl = (int)HeroesHelper.GetSpellTier(_view.stats.MergeTier);
            var hero = _view.GetComponent<IHeroController>();
            fx.transform.SetPositionAndRotation(_view.transform.position, _view.transform.rotation);
            fx.Launch(lvl, _config.cellsMasksByTear[lvl], _view.stats.SpellPower.Get(), 
                _config.speed, hero.Battle.GetTeam(hero.TeamNum).enemyUnits);
        }

        private CrescentSlashView GetFxView()
        {
            if (_fxView != null) return _fxView;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_CrescentSlash);
            var instance = Object.Instantiate(prefab).GetComponent<CrescentSlashView>();
            _fxView = instance;
            return instance;
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