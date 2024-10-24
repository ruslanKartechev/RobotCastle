using System.Collections.Generic;
using System.Threading;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellBlizzard : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public float BaseSpellPower => _config.spellDamage[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
        
        public string name => "spell";
        
        public int order => 10;
        
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellBlizzard(HeroComponents view, SpellConfigBlizzard config)
        {
            _components = view;
            _config = config;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _isActive = true;
            CLog.Log($"[{_components.gameObject.name}] [{nameof(SpellBlizzard)}]");
            _manaAdder.CanAdd = false;
            _components.attackManager.OnAttackStep -= OnAttack;
            _components.attackManager.OnAttackStep += OnAttack;
        }
        
        public void Stop()
        {
            _components.attackManager.OnAttackStep -= OnAttack;
            _isActive = true;
            _manaAdder.CanAdd = true;
            _token?.Cancel();
        }
        
        private SpellConfigBlizzard _config;
        private SpellParticleOnGridEffect _fxView;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        
        private void OnAttack()
        {
            _components.attackManager.OnAttackStep -= OnAttack;
            _components.stats.ManaResetAfterFull.Reset(_components);
            var target = _components.attackManager.LastTarget.GetGameObject();
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var enemies = HeroesManager.GetHeroesEnemies(_components);
            var map = _components.agent.Map;
            var (affectedEnemies, cells) = HeroesManager.GetCellsHeroesInsideCellMask(_config.cellsMasksByTear[lvl], 
                target.transform.position, map, enemies);
            foreach (var hero in affectedEnemies)
            {
                _components.damageSource.DamageSpellAndPhys(hero.Components.damageReceiver);
                hero.SetBehaviour(new HeroStunnedBehaviour(_config.duration[lvl]));
            }
            var view = GetFxView();
            view.transform.position = target.transform.position;
            var worldPositions = new List<Vector3>(cells.Count);
            foreach (var enemy in affectedEnemies)
                worldPositions.Add(map.GetWorldFromCell(enemy.Components.state.currentCell));
            view.Show(worldPositions);
            
            _isActive = false;
            _manaAdder.CanAdd = true;
        }

        
        private SpellParticleOnGridEffect GetFxView()
        {
            if (_fxView != null) return _fxView;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Blizzard);
            var instance = Object.Instantiate(prefab).GetComponent<SpellParticleOnGridEffect>();
            _fxView = instance;
            return instance;
        }
        
      

    }
}