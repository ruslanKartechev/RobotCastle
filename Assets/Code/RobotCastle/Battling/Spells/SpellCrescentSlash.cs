﻿using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellCrescentSlash : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public const int MaxDistance = 10;
        
        public float BaseSpellPower => _config.spellDamage[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
        public string name => "spell";
        public int order => 10;
        
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellCrescentSlash(HeroComponents view, SpellConfigCrescentSlash config)
        {
            _components = view;
            _config = config;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(view);
            _components.stats.SpellPower.AddPermanentDecorator(this);
        }


        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            CLog.Log($"[{_components.gameObject.name}] [{nameof(SpellCrescentSlash)}]");
            _isActive = true;
            _manaAdder.CanAdd = false;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _components.processes.Add(this);
            CheckingPosition(_token.Token);
        }
        
        public void Stop()
        {
            _isActive = false;
            _token?.Cancel();
        }

        private SpellConfigCrescentSlash _config;
        private CrescentSlashView _fxView;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        
        private void Launch()
        {
            _manaAdder.CanAdd = true;
            _isActive = false;
            _components.processes.Remove(this);
            _components.stats.ManaResetAfterFull.Reset(_components);
            var fx = GetFxView();
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var hero = _components.GetComponent<IHeroController>();
            fx.transform.SetPositionAndRotation(_components.transform.position, _components.transform.rotation);
            fx.Launch(lvl, _config.cellsMasksByTear[lvl], _config.speed, 
                hero.Battle.GetTeam(hero.TeamNum).enemyUnits, _components.damageSource);
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
            
            var map = _components.agent.Map;
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var mask = _config.cellsMasksByTear[lvl];
            var enemies = HeroesManager.GetHeroesEnemies(_components);
            while (token.IsCancellationRequested == false)
            {
                var tr = _components.transform;
                var remainder = Mathf.Abs(tr.eulerAngles.y) % 90f;
                if (remainder < 2)
                {
                    var frw = tr.forward;
                    var cellCenter = _components.agent.CurrentCell;
                    var frwCellDir = new Vector2Int(Mathf.RoundToInt(frw.x), Mathf.RoundToInt(frw.z));
                    for (var stepInd = 0; stepInd < MaxDistance; stepInd++)
                    {
                        if (HeroesManager.CheckIfAtLeastOneHeroInMask(mask, cellCenter, map, enemies))
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