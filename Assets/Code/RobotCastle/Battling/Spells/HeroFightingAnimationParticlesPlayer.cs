using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroFightingAnimationParticlesPlayer : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _particlesPerAnimation;

        public void AE_HitParticle1()
        {
            PlayByIndex(0);
        }
        
        public void AE_HitParticle2()
        {
            PlayByIndex(1);
        }
        
        public void AE_HitParticle3()
        {
            PlayByIndex(2);
        }

        private void PlayByIndex(int index)
        {
            if (index >= _particlesPerAnimation.Count) return;
            _particlesPerAnimation[index].gameObject.SetActive(true);
            _particlesPerAnimation[index].Play();
        }

    }
}