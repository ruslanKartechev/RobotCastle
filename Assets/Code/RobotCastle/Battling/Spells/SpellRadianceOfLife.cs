using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellRadianceOfLife : Spell, IFullManaListener, IStatDecorator
    {
        public float BaseSpellPower => _config.healAmount[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
        
        public string name => "spell";
        public int order => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellRadianceOfLife(HeroComponents view, SpellConfigRadianceOfLife config)
        {
            _components = view;
            _config = config;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        private SpellConfigRadianceOfLife _config;
        private SpellParticlesOnHero _fxView;
        private ConditionedManaAdder _manaAdder;

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            CLog.Log($"[{_components.gameObject.name}] [{nameof(SpellRadianceOfLife)}]");
            _isActive = true;
            _components.stats.ManaResetAfterFull.Reset(_components);
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var amount = _components.stats.SpellPower.Get();
            var allies = HeroesManager.GetHeroesAllies(_components);
            var map = _components.agent.Map;
            var (heroesAffected, cells) = HeroesManager.GetCellsHeroesInsideCellMask(_config.cellsMasksByTear[lvl], 
                _components.transform.position, map, allies);
            
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
            effect.ShowTrackingDefaultDuration(_components.transform);
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