using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellJudgementOfLight : IFullManaListener, IStatDecorator, IHeroProcess
    {
        public float BaseSpellPower => _config.spellDamage[(int)HeroesHelper.GetSpellTier(_view.stats.MergeTier)];
        public string name => "spell";
        public int priority => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }

        public SpellJudgementOfLight(HeroView view, SpellConfigJudgementOfLight config)
        {
            _view = view;
            _config = config;
            view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(view);
            _view.stats.SpellPower.PermanentDecorators.Add(this);
        }
        
        private SpellConfigJudgementOfLight _config;
        private HeroView _view;
        private SpellParticleOnGridEffect _fxView;
        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private bool _isActive;

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _isActive = true;
            _token = new CancellationTokenSource();
            _view.processes.Add(this);
            _manaAdder.CanAdd = false;
            Waiting(_token.Token);
        }

        private async void Waiting(CancellationToken token)
        {
            var lvl = (int)HeroesHelper.GetSpellTier(_view.stats.MergeTier);
            var mask = _config.cellsMasksByTear[lvl];
            var allEnemies = HeroesHelper.GetHeroesEnemies(_view);
            var map = _view.agent.Map;

            const int waitMs = (int)(.25f * 1000);
            while (!token.IsCancellationRequested)
            {
                var (affectedEnemies, cells) =
                    HeroesHelper.GetCellsHeroesInsideCellMask(mask, _view.transform.position, map, allEnemies);
                if (affectedEnemies.Count > 0)
                {
                    var worldPositions = new List<Vector3>(cells.Count);
                    foreach (var c in cells)
                        worldPositions.Add(map.GetWorldFromCell(c));
                    var effect = GetFxView();
                    effect.Show(worldPositions);
                    var args = new DamageArgs(_config.physDamage[lvl], _view.stats.SpellPower.Get());
                    for (var i = allEnemies.Count - 1; i >= 0; i--)
                        allEnemies[i].View.damageReceiver.TakeDamage(args);
                    _isActive = false;
                    _view.stats.ManaResetAfterFull.Reset(_view);
                    _view.processes.Remove(this);
                    _manaAdder.CanAdd = true;
                    return;
                }
                await Task.Delay(waitMs, token);
            }
        }

        private SpellParticleOnGridEffect GetFxView()
        {
            if (_fxView != null) return _fxView;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_JudgementOfLight);
            var instance = Object.Instantiate(prefab).GetComponent<SpellParticleOnGridEffect>();
            _fxView = instance;
            return instance;
        }

        public void Stop()
        {
            _manaAdder.CanAdd = true;
            _isActive = false;
            _token?.Cancel();
        }
    }
}