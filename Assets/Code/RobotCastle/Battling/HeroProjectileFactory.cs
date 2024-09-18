using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroProjectileFactory : MonoBehaviour, IProjectileFactory
    {
        [SerializeField] private string _projId = "bullet";
        [SerializeField] private string _projPrefab = "prefabs/projectiles/bullet";
        [SerializeField] private int _startCount = 4;

        private void Start()
        {
            var pools = ServiceLocator.Get<ISimplePoolsManager>();
            pools.AddPoolIfNot(_projId, _projPrefab, _startCount);
        }

        public IProjectile GetProjectile()
        {
            var pools = ServiceLocator.Get<ISimplePoolsManager>();
            return (IProjectile)pools.GetOne(_projId);
        }
    }
}