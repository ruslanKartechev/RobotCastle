using System.Threading;
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
        
        public SpellCrescentSlash(HeroComponents components, SpellConfigCrescentSlash config)
        {
            _components = components;
            _config = config;
            _hero = components.gameObject.GetComponent<IHeroController>();
            Setup(config, out _manaAdder);
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
            if (_isActive)
            {
                _components.animationEventReceiver.OnAttackEvent -= OnAnimationEvent;
                _manaAdder.CanAdd = true;
                _components.processes.Remove(this);
                _isActive = false;
                _token?.Cancel();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        private IHeroController _hero;
        private SpellConfigCrescentSlash _config;
        private CrescentSlashView _fxView;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        

        private async void CheckingPosition(CancellationToken token)
        {
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            var map = _components.movement.Map;
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var mask = _config.cellsMasksByTear[lvl];
            var enemies = HeroesManager.GetHeroesEnemies(_components);
            var framesInside = 0;
            const int minFrames = 3;
            while (token.IsCancellationRequested == false)
            {
                var tr = _components.transform;
                var remainder = Mathf.Abs(tr.eulerAngles.y) % 90f;
                if (remainder < 2)
                {
                    var frw = tr.forward;
                    var cellCenter = _components.state.currentCell;
                    var frwCellDir = new Vector2Int(Mathf.RoundToInt(frw.x), Mathf.RoundToInt(frw.z));
                    for (var stepInd = 0; stepInd < MaxDistance; stepInd++)
                    {
                        if (HeroesManager.CheckIfAtLeastOneHeroInMask(mask, cellCenter, map, enemies))
                            framesInside++;
                        else
                            framesInside = 0;

                        if (framesInside >= minFrames)
                        {
                            Animate();
                            return;
                        }

                        cellCenter += frwCellDir;
                    }
                }
                else
                    framesInside = 0;
                
                await Task.Yield();
                await Task.Yield();
            }
        }

        private void Animate()
        {
            if (!_isActive) return;
            _hero.PauseCurrentBehaviour();
            _components.animationEventReceiver.OnAttackEvent += OnAnimationEvent;
            _components.animator.Play("Cast",0,0);
        }
        
        private async void OnAnimationEvent()
        {
            if (_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
            Launch();
            _components.animationEventReceiver.OnAttackEvent -= OnAnimationEvent;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            DelayedComplete(_token.Token);
        }

        private async void DelayedComplete(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            await HeroesManager.WaitGameTime(_config.animaDelaySec, _token.Token);
            Complete();
            _hero.ResumeCurrentBehaviour();
        }

        private void Launch()
        {
            if (!_isActive) return;
          
            var fx = GetFxView();
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var hero = _components.GetComponent<IHeroController>();
            fx.transform.SetPositionAndRotation(_components.transform.position, _components.transform.rotation);
            fx.Launch(lvl, _config.cellsMasksByTear[lvl], _config.speed, 
                hero.Battle.GetTeam(hero.TeamNum).enemyUnits, _components.damageSource);
        }

        private void Complete()
        {
            _manaAdder.CanAdd = true;
            _isActive = false;
            _components.processes.Remove(this);
            _components.stats.ManaResetAfterFull.Reset(_components);
        }
   
        private CrescentSlashView GetFxView()
        {
            if (_fxView != null) return _fxView;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_CrescentSlash);
            var instance = Object.Instantiate(prefab).GetComponent<CrescentSlashView>();
            _fxView = instance;
            return instance;
        }

    
    }
}