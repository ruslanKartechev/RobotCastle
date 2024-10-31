using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellHeadshot : Spell, IFullManaListener, IStatDecorator, IProjectileFactory, IHeroProcess
    {
        public SpellHeadshot(HeroComponents components, SpellConfigHeadshot config)
        {
            _config = config;
            _components = components;
            _damageMod = new DamageModAddAmount(0, 0);
            _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        public float BaseSpellPower => _config.spellDamage[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
        public string name => "spell";
        public int order => 10;
        public float Decorate(float val) => val + BaseSpellPower;
        
        public void OnFullMana(GameObject heroGo)
        {
            Execute();
        }

        public IProjectile GetProjectile()
        {
            var prefab = Resources.Load<GameObject>("prefabs/projectiles/bullet_headshot_spell");
            var instanceGo = Object.Instantiate(prefab);
            instanceGo.SetActive(false);
            return instanceGo.GetComponent<IProjectile>();
        }
        
        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _manaAdder.CanAdd = true;
                _components.attackManager.OnAttackStep -= OnAttack;
                var atk = ((HeroRangedAttackManager)_components.attackManager);
                if (atk && atk.ProjectileFactory == this)
                    atk.ProjectileFactory = _prevFactory;
                _token.Cancel();

            }
        }
        
        private SpellConfigHeadshot _config;
        private DamageModAddAmount _damageMod;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        private bool _isActive;
        private IProjectileFactory _prevFactory;

        private void Execute()
        {
            if (_isActive)
                return;
            _isActive = true;
            _components.processes.Add(this);
            CLog.LogWhite($"[{_components.gameObject.name}] Spell headshot executed");
            var atk = ((HeroRangedAttackManager)_components.attackManager);
            if (atk == null)
                throw new System.Exception($"{nameof(SpellHeadshot)}  cannot cast AttackManager to HeroRangedAttackManager");
            atk.OnHit -= OnHit;
            atk.OnHit += OnHit;
            atk.OnAttackStep += OnAttack;
            _manaAdder.CanAdd = false;
            _damageMod.addedMagicDamage = _components.stats.SpellPower.Get();
            _damageMod.addedPhysDamage = _config.physDamage[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
            _components.damageSource.AddModifier(_damageMod);
            _prevFactory = atk.ProjectileFactory;
            atk.ProjectileFactory = this;
        }

        private void OnAttack()
        {
            _components.attackManager.OnAttackStep -= OnAttack;
            if(_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
        }

        private void OnHit()
        {
            _token = new CancellationTokenSource();
            DelayedEnd(_token.Token);
        }

        private async void DelayedEnd(CancellationToken token)
        {
            await Task.Yield();            
            await Task.Yield();
            if (token.IsCancellationRequested || !_isActive) return;
            var atk = ((HeroRangedAttackManager)_components.attackManager);
            atk.OnHit -= OnHit;
            _components.damageSource.RemoveModifier(_damageMod);
            _components.stats.ManaResetAfterFull.Reset(_components);
            _isActive = false;
            _manaAdder.CanAdd = true;
            atk.ProjectileFactory = _prevFactory;
            _components.processes.Remove(this);
        }

    
    }
}