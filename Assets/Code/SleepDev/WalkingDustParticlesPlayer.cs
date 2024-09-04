using UnityEngine;

namespace SleepDev
{
    public class WalkingDustParticlesPlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleLeft;
        [SerializeField] private ParticleSystem _particleRight;

        public void OnLeft()
        {
            _particleLeft.Play();
        }

        public void OnRight()
        {
            _particleRight.Play();
        }
        
    }
}