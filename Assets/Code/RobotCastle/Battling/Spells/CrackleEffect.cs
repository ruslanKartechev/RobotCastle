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
            if (_startParticles != null)
            {
                _startParticles.gameObject.SetActive(true);
                _startParticles.transform.parent = transform.parent;
                _startParticles.transform.position = startPoint;
            }
            var mat = _lineRenderer.material;
            SetAlpha(1f);            
            foreach (var p in _hitParticles)
                p.gameObject.SetActive(false);
            var positions = new List<Vector3>(10);
            var elapsed = 0f;
            _lineRenderer.enabled = true;
            var particleInd = 0;
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

                positions.Clear();
                positions.Add(startPoint);
                for (var k = 0; k < _affectedTargets.Count; k++)
                {
                    var tt = _affectedTargets[k];
                    if(tt.IsDead)
                        continue;
                    var pos = tt.Components.transform.position + Vector3.up;
                    positions.Add(pos);
                }
                _lineRenderer.positionCount = positions.Count;
                _lineRenderer.SetPositions(positions.ToArray());

                await Task.Yield();
                if (token.IsCancellationRequested) return;
                
                transform.position = target.position;
                hero.Components.damageSource.DamageSpellAndPhys(damagePhys, damageSpell, enemy.Components.damageReceiver);
                var particles = _hitParticles[particleInd];
                particles.gameObject.SetActive(true);
                particles.transform.position = target.position + Vector3.up;
                particleInd++;
            }
            elapsed = 0f;
            while (!token.IsCancellationRequested && elapsed < _stayTime)
            {
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            elapsed = 0f;
            while (!token.IsCancellationRequested && elapsed < _hideTime)
            {
                var a = Mathf.Lerp(1f, 0f, elapsed / _hideTime);
                SetAlpha(a);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            Hide();

            void SetAlpha(float a)
            {
                var col = mat.GetColor(BaseColor);
                col.a = a;
                mat.SetColor(BaseColor, col);

            }
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
            // _particle.Play();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        [SerializeField] private float _stayTime = 2f;
        [SerializeField] private float _hideTime = .5f;
        [SerializeField] private ParticleSystem _startParticles;
        [SerializeField] private List<ParticleSystem> _hitParticles;
        [SerializeField] private LineRenderer _lineRenderer;
        
        private List<IHeroController> _affectedTargets = new (5);
        private static readonly int BaseColor = Shader.PropertyToID("_Color");
    }
}