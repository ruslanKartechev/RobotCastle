using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class CastleHealthView : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _healthPoles;
        private int _current;
        
        public void SetHealth(int totalHealth)
        {
            _current = totalHealth;
            for (var i = 0; i < totalHealth; i++)
                _healthPoles[i].SetActive(true);
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
                _healthPoles[i].SetActive(false);
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
            for (var i = _current; i < totalHealth; i++)
                _healthPoles[i].SetActive(true);
            _current = totalHealth;
        }
    }
}