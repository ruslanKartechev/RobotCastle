using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellRadianceOfLife : Spell, IFullManaListener, IStatDecorator
    {
        public float BaseSpellPower => _config.healAmount[(int)HeroesManager.GetSpellTier(_view.stats.MergeTier)];
        
        public string name => "spell";
        public int order => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellRadianceOfLife(HeroComponents view, SpellConfigRadianceOfLife config)
        {
            _view = view;
            _config = config;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_view);
            _view.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        private SpellConfigRadianceOfLife _config;
        private SpellParticlesOnHero _fxView;
        private ConditionedManaAdder _manaAdder;

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            CLog.Log($"[{_view.gameObject.name}] [{nameof(SpellRadianceOfLife)}]");
            _isActive = true;
            _view.stats.ManaResetAfterFull.Reset(_view);
            var lvl = (int)HeroesManager.GetSpellTier(_view.stats.MergeTier);
            var amount = _view.stats.SpellPower.Get();
            var allies = HeroesManager.GetHeroesAllies(_view);
            var map = _view.agent.Map;
            var (heroesAffected, cells) = HeroesManager.GetCellsHeroesInsideCellMask(_config.cellsMasksByTear[lvl], 
                _view.transform.position, map, allies);
            
            var worldPositions = new List<Vector3>(cells.Count);
            // CLog.LogRed($"Affected count: {heroesAffected.Count}");
            foreach (var hero in heroesAffected)
            {
                var val = hero.Components.stats.HealthCurrent.Val;
                val += amount;
                hero.Components.stats.HealthCurrent.SetBaseAndCurrent(val);
                worldPositions.Add(hero.Components.transform.position);
            }
            var effect = GetFxView();
            effect.Show(_view.transform);
            _isActive = false;
        }
        
        private SpellParticlesOnHero GetFxView()
        {
            if (_fxView != null) return _fxView;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_RadianceOfLife);
            var instance = Object.Instantiate(prefab).GetComponent<SpellParticlesOnHero>();
            _fxView = instance;
            return instance;
        }
    }
}