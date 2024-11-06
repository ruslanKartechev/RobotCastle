using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class CrackleEffect : MonoBehaviour
    {
        public async Task FlyAndHit(Vector3 startPoint, int damagePhys, int damageSpell, int jumps, IHeroController hero, CancellationToken token)
        {
            _affectedTargets.Clear();
            transform.position = startPoint;
            for (var i = 0; i < jumps+1 && !token.IsCancellationRequested; i++)
            {
                var enemy = GetClosestEnemy(hero);
                if (enemy == null)
                {
                    CLog.Log("No more enemies");
                    Hide();
                    return;
                }
                _affectedTargets.Add(enemy);
                var target = enemy.Components.transform;
                var startPos = transform.position;
                var time = (target.position - transform.position).magnitude / _moveSpeed;
                var elapsed = 0f;
                while (!token.IsCancellationRequested && elapsed < time)
                {
                    transform.position = Vector3.Lerp(startPos, target.position, elapsed / time);
                    elapsed += Time.deltaTime;
                    await Task.Yield();
                }
                if (token.IsCancellationRequested)
                    return;
                transform.position = target.position;
                hero.Components.damageSource.DamageSpellAndPhys(damagePhys, damageSpell, enemy.Components.damageReceiver);
                if (_hitParticle != null)
                {
                    _hitParticle.gameObject.SetActive(true);
                    _hitParticle.Play();
                }
            }
            if (token.IsCancellationRequested)
                return;
            Hide();
        }

        private IHeroController GetClosestEnemy(IHeroController hero)
        {
            var enemies = hero.Battle.GetTeam(hero.TeamNum).enemyUnits;
            if (enemies.Count == 0)
                return null;
            var result = (IHeroController)null;
            var minD2 = float.MaxValue;
            var mPos = transform.position;
            foreach (var en in enemies)
            {
                if (_affectedTargets.Contains(en))
                    continue;
                var d2 = (mPos - en.Components.transform.position).sqrMagnitude;
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    result = en;
                }
            }

            if (result == null && _affectedTargets.Count > 1)
            {
                foreach (var en in _affectedTargets)
                {
                    var d2 = (mPos - en.Components.transform.position).sqrMagnitude;
                    if (d2 <= minD2)
                    {
                        minD2 = d2;
                        result = en;
                    }
                }
            }
            
            return result;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _particle.Play();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        [SerializeField] private float _moveSpeed;
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private ParticleSystem _hitParticle;
        private List<IHeroController> _affectedTargets = new (5);

    }
}