﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellParticlesByLevel : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _particles;
        [SerializeField] private float _duration = 1.5f;
        
        public async void Show(int level)
        {
            gameObject.SetActive(true);
            for (var i = 0; i < _particles.Count; i++)
            {
                var p = _particles[i];
                p.gameObject.SetActive(i == level);
                if (i == level)
                {
                    p.gameObject.SetActive(true);
                    p.Play();
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }
            await Task.Delay((int)(1000 * _duration));
            if (Application.isPlaying == false)
                return;
            gameObject.SetActive(false);
        }
        
    }
}