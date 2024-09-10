using UnityEngine;

namespace Bomber
{
    public class MapCellBreakableWall : MapCellContent
    {
        [SerializeField] private ParticleSystem _brokenParticles;
        [SerializeField] private Collider _collider;
        private bool _isAlive;


        public bool IsAlive => _isAlive;
        

        public bool CanDamage() => true;

        public bool WillDie(byte damageReceived) => true;

        public void TakeDamage(byte damage, Vector3 position)
        {
     
        }

    }
}