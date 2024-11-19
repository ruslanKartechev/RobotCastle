using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellBackstab : Spell, IFullManaListener, IHeroProcess
    {
        public SpellBackstab(SpellConfigBackstab config, HeroComponents components)
        {
            _config = config;
            _components = components;
            Setup(config, out _manaAdder);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _hero = components.gameObject.GetComponent<IHeroController>();
            InitFx();
        }
     
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token = new CancellationTokenSource();
            _components.processes.Add(this);
            Work(_token.Token);
        }

        public void Stop()
        {
            if (_isAttacking)
            {
                _components.attackManager.OnAttackStep -= OnAttack;
                _isAttacking = false;
            }
            if (_isActive)
            {
                _token?.Cancel();
                _isActive = false;
                _manaAdder.CanAdd = true;
            }
        }

        private const float MoveTime = .3f;
        private CancellationTokenSource _token;
        private SpellConfigBackstab _config;
        private ConditionedManaAdder _manaAdder;
        private SpellParticlesByLevel _fx1;
        private SpellParticlesByLevel _fx2;

        private IHeroController _hero;
        private bool _isAttacking;
        
        private async void Work(CancellationToken token)
        {
            await Task.Yield();
            if (token.IsCancellationRequested)
                return;
            _isActive = true;
            _manaAdder.CanAdd = false;
            _components.stats.ManaResetAfterFull.Reset(_components);
            var map = _components.movement.Map;
            var canTp = false;
            var maskRef = new CellsMask() {
                mask = new List<Vector2Int>(1){new(0,-1)}
            };
            var rotMask = new CellsMask() {
                mask = new List<Vector2Int>(1) { new(0, -1) }
            };
            while (!token.IsCancellationRequested && !canTp)
            {
                var enemy = await WaitForTarget(token);
                if (token.IsCancellationRequested) return;
                var cellFree = false;
                Vector2Int cell;
                do
                {
                    if (enemy.IsDead)
                    {
                        canTp = false;
                        break;
                    }
                    var rot = enemy.Components.transform.rotation;
                    rotMask.SetAsRotated(maskRef, rot);
                    cell = rotMask.mask[0] + enemy.Components.state.currentCell;
                    cellFree = !map.IsOutOfBounce(cell);
                    if (map.IsFullyFree(cell))
                    {
                        canTp = true;
                        TeleportToAttack(cell, enemy, token);
                        return;
                    }
                    await Task.Delay(150, token);
                    if (token.IsCancellationRequested)
                        return;
                } while (!cellFree && !token.IsCancellationRequested);
            }

            bool IsCellFree(Vector2Int cell)
            {
                foreach (var agent in map.ActiveAgents)
                {
                    if (agent.CurrentCell == cell)
                        return false;
                }
                return true;
            }
        }

        private async void TeleportToAttack(Vector2Int cell, IHeroController enemy, CancellationToken token)
        {
            _isAttacking = true;
            _hero.StopCurrentBehaviour();
            _components.movement.Stop();
            _components.animator.Play("Idle", 0, 0);
            _components.movement.TargetCell = cell;
            _fx1.PlayLevelAtPoint(_components.transform.position, 0);
            if (_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
            
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            var lowPos = _components.transform.position;
            lowPos.y = -100;
            _components.transform.position = lowPos;
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            var map = _components.movement.Map;
            var worldPos = map.GetWorldFromCell(cell);
            var rot = Quaternion.LookRotation(enemy.Components.transform.position - worldPos);
            var tr = _components.transform;
            tr.rotation = rot;
            var elapsed = 0f;
            while (elapsed < MoveTime && !token.IsCancellationRequested)
            {
                tr.position = Vector3.Lerp(lowPos, worldPos, elapsed / MoveTime);                
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            _components.animator.Play("Cast", 0,0);
            tr.position = worldPos;
            _components.movement.SyncCellToWorldPos();
            _fx2.PlayLevelAtPoint(_components.transform.position, 0);

            _components.attackManager.OnAttackStep += OnAttack;
            _components.attackManager.BeginAttack(enemy.Components.damageReceiver);
        }

        private void OnAttack()
        {
            if (_isActive == false)
                return;
            _components.attackManager.OnAttackStep -= OnAttack;
            _components.damageSource.DamageSpell(_components.attackManager.LastTarget);
            _components.attackManager.Stop();
            _hero.SetBehaviour(new HeroAttackEnemyBehaviour());
            Complete();
        }

        private void Complete()
        {
            _manaAdder.CanAdd = true;
            _isAttacking = true;
            _isActive = false;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
        }

        private async Task<IHeroController> WaitForTarget(CancellationToken token)
        {
            var target = BattleManager.GetBestTargetForAttack(_hero, null);
            while (target is not {Count: > 0} && !token.IsCancellationRequested)
                await Task.Delay(150, token);
            if (token.IsCancellationRequested) return null;
            return target[0];
        }

        private void InitFx()
        {
            var prefab = Resources.Load<SpellParticlesByLevel>(HeroesConstants.SpellFXPrefab_Backstab);
            _fx1 = Object.Instantiate(prefab);
            _fx2 = Object.Instantiate(prefab);
            _fx1.gameObject.SetActive(false);
            _fx2.gameObject.SetActive(false);
        }

    
    }

 
}