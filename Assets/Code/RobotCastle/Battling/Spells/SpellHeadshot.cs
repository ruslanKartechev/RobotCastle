using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellHeadshot : Spell, IFullManaListener, 
        IProjectileFactory, IHeroProcess, IAttackHitAction, IAttackAction
    {
        public SpellHeadshot(HeroComponents components, SpellConfigHeadshot config)
        {
            _config = config;
            _components = components;
            _damageMod = new DamageModAddAmount(0, 0);
            _manaAdder = new ConditionedManaAdder(_components);
            Setup(config, out _manaAdder);
            // _components.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        public float BaseSpellPower => _config.spellDamage[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
        public string name => "spell";
        public int order => 10;
        public float Decorate(float val) => val + BaseSpellPower;
        
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _isActive = true;
            _components.processes.Add(this);
            _token?.Cancel();
            _token = new CancellationTokenSource();
            Working(_token.Token);
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
                var atk = _components.attackManager;
                if (atk != null)
                {
                    atk.HitAction = _prevHitAction;
                    atk.AttackAction = _prevAttackAction;
                }
                _token.Cancel();
            }
        }
        
        private SpellConfigHeadshot _config;
        private DamageModAddAmount _damageMod;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        private bool _isActive;
        private bool _isWaiting;
        private IAttackHitAction _prevHitAction;
        private IAttackAction _prevAttackAction;
        

        private async void Working(CancellationToken token)
        {
            CLog.LogWhite($"[{_components.gameObject.name}] Spell headshot executed");
            var atk = ((HeroRangedAttackManager)_components.attackManager);
            if (atk == null)
                throw new System.Exception($"{nameof(SpellHeadshot)}  cannot cast AttackManager to HeroRangedAttackManager");
            
            _prevHitAction = atk.HitAction;
            _prevAttackAction = atk.AttackAction;

            atk.HitAction = this;
            atk.AttackAction = this;
            
            _isWaiting = true;
            _manaAdder.CanAdd = false;
            while (!token.IsCancellationRequested && _isWaiting)
                await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            atk.HitAction = _prevHitAction;
            atk.AttackAction = _prevAttackAction;
            _components.damageSource.RemoveModifier(_damageMod);
            _components.stats.ManaResetAfterFull.Reset(_components);
            _isActive = false;
            _manaAdder.CanAdd = true;
            _components.processes.Remove(this);
        }

        private void OnAttack()
        {
            _components.attackManager.OnAttackStep -= OnAttack;
            if(_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
        }

        public void Hit(object target)
        {
            _damageMod.addedMagicDamage = _components.stats.SpellPower.Get();
            _damageMod.addedPhysDamage = _config.physDamage[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
            _components.damageSource.AddModifier(_damageMod);
            
            var dm = (IDamageReceiver)target;
            if (dm != null)
            {
                _components.damageSource.DamagePhys(dm);
                _components.damageSource.DamageSpell(dm);
            }
            
            _components.damageSource.RemoveModifier(_damageMod);
            _isWaiting = false;
        }

        public void Attack(IDamageReceiver target, int animationIndex)
        {
            if(_components.shootParticles != null)
                _components.shootParticles.Play();

            var pp = animationIndex == 0 ? _components.projectileSpawnPoint : _components.projectileSpawnPoint2;
            
            GetProjectile().LaunchProjectile(pp, target.GetGameObject().transform, 
                _components.stats.ProjectileSpeed, Hit, target);

            if (_components.attackSounds.Count > 0)
            {
                var s = _components.attackSounds.Random();
                s.Play();
            }
        }
    }
}