using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class CastleHealthView : MonoBehaviour
    {
        [SerializeField] private float _scaleTime = .3f;
        [SerializeField] private List<Animator> _healthPoles;
        private int _current;
        
        public void SetHealth(int totalHealth)
        {
            _current = totalHealth;
            for (var i = 0; i < totalHealth; i++)
            {
                _healthPoles[i].gameObject.SetActive(true);
                _healthPoles[i].transform.localScale = Vector3.one;
                _healthPoles[i].Play("Idle");
            }
        }

        public void MinusHealth(int totalHealth)
        {
            var difference = _current - totalHealth;
            if (difference < 0)
            {
                CLog.LogError("Error. Removed health cannot be < 0");
                return;
            }

            for (var i = totalHealth; i < _healthPoles.Count; i++)
            {
                _healthPoles[i].SetTrigger("Break");
            }
            _current = totalHealth;
        }

        public void AddHealth(int totalHealth)
        {
            var added = totalHealth - _current;
            if (added < 0)
            {
                CLog.LogError("Error. Added health cannot be < 0");
                return;
            }
            var count = _healthPoles.Count;
            for (var i = _current; i < totalHealth && i < count ; i++)
            {
                _healthPoles[i].SetTrigger("Repair");
            }
            _current = totalHealth;
        }
        
    }
}