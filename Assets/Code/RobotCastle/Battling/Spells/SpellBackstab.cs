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
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
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
        
        private CancellationTokenSource _token;
        private SpellConfigBackstab _config;
        private ConditionedManaAdder _manaAdder;
        private SpellParticlesOnHero _fx1;
        private SpellParticlesOnHero _fx2;

        private IHeroController _hero;
        private bool _isAttacking;

        
        private async void Work(CancellationToken token)
        {
            await Task.Yield();
            await Task.Yield();
            if (token.IsCancellationRequested)
                return;
            _isActive = true;
            _manaAdder.CanAdd = false;
            _components.stats.ManaResetAfterFull.Reset(_components);
            var map = _components.agent.Map;
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
                    if (cellFree)
                    {
                        cellFree &= map.Grid[cell.x, cell.y].isPlayerWalkable;
                        cellFree &= IsCellFree(cell);
                        if (cellFree)
                        {
                            canTp = true;
                            TeleportToAttack(cell, enemy, token);
                            return;
                        }
                    }
                    await Task.Delay(150, token);
                    if (token.IsCancellationRequested)
                        return;
                } while (!cellFree);
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
            _components.animator.Play("Idle");
            _fx1.PlayHitParticles();
            if (_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            var map = _components.agent.Map;
            var worldPos = map.GetWorldFromCell(cell);
            var rot = Quaternion.LookRotation(enemy.Components.transform.position - worldPos);
            // Debug.DrawLine(_components.transform.position + Vector3.up * .5f, 
            //     worldPos + Vector3.up * .5f, Color.red, 10f);
            //
            // Debug.DrawLine(worldPos + Vector3.up * .5f, 
            //     enemy.Components.transform.position + Vector3.up * .5f, Color.blue, 10f);
            //
            _components.transform.SetPositionAndRotation(worldPos, rot);
            _components.agent.SetCurrentCellFromWorldPosition();
            
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            _fx2.PlayHitParticles();
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
            Complete();
        }

        private void Complete()
        {
            _manaAdder.CanAdd = true;
            _isAttacking = true;
            _isActive = false;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
            _hero.SetBehaviour(new HeroAttackEnemyBehaviour());
        }

        private async Task<IHeroController> WaitForTarget(CancellationToken token)
        {
            var target = BattleManager.GetBestTargetForAttack(_hero);
            while (target is not {Count: > 0} && !token.IsCancellationRequested)
                await Task.Delay(150, token);
            if (token.IsCancellationRequested) return null;
            return target[0];
        }

        private void InitFx()
        {
            var prefab = Resources.Load<SpellParticlesOnHero>(HeroesConstants.SpellFXPrefab_Backstab);
            _fx1 = Object.Instantiate(prefab);
            _fx2 = Object.Instantiate(prefab);
            _fx1.gameObject.SetActive(false);
            _fx2.gameObject.SetActive(false);
        }

    
    }

 
}