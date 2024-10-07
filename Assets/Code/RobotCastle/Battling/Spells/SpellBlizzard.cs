using System.Collections.Generic;
using System.Threading;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellBlizzard : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public float BaseSpellPower => _config.spellDamage[(int)HeroesManager.GetSpellTier(_view.stats.MergeTier)];
        public string name => "spell";
        public int priority => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellBlizzard(HeroView view, SpellConfigBlizzard config)
        {
            _view = view;
            _config = config;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_view);
            _view.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        private SpellConfigBlizzard _config;
        private SpellParticleOnGridEffect _fxView;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _isActive = true;
            CLog.Log($"[{_view.gameObject.name}] [{nameof(SpellBlizzard)}]");
            _manaAdder.CanAdd = false;
            _view.attackManager.OnAttackStep += OnAttack;
        }

        private void OnAttack()
        {
            _view.attackManager.OnAttackStep -= OnAttack;
            _view.stats.ManaResetAfterFull.Reset(_view);
            var target = _view.attackManager.LastTarget.GetGameObject();
            var lvl = (int)HeroesManager.GetSpellTier(_view.stats.MergeTier);
            var enemies = HeroesManager.GetHeroesEnemies(_view);
            var map = _view.agent.Map;
            var (heroesAffected, cells) = HeroesManager.GetCellsHeroesInsideCellMask(_config.cellsMasksByTear[lvl], 
                target.transform.position, map, enemies);
            var args = new DamageArgs(_config.physDamage[lvl], _view.stats.SpellPower.Get());
            foreach (var hero in heroesAffected)
            {
                hero.View.damageReceiver.TakeDamage(args);
                hero.SetBehaviour(new HeroStunnedBehaviour(_config.duration[lvl]));
            }
            var view = GetFxView();
            view.transform.position = target.transform.position;
            var worldPositions = new List<Vector3>(cells.Count);
            foreach (var enemy in heroesAffected)
                worldPositions.Add(map.GetWorldFromCell(enemy.View.state.currentCell));
            view.Show(worldPositions);
            
            _isActive = false;
            _manaAdder.CanAdd = true;
        }

        public void Stop()
        {
            _isActive = true;
            _manaAdder.CanAdd = true;
            _token?.Cancel();
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