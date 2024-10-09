using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class CastleHealthView : MonoBehaviour
    {
        [SerializeField] private float _scaleTime = .3f;
        [SerializeField] private List<GameObject> _healthPoles;
        private int _current;
        
        public void SetHealth(int totalHealth)
        {
            _current = totalHealth;
            for (var i = 0; i < totalHealth; i++)
            {
                _healthPoles[i].SetActive(true);
                _healthPoles[i].transform.localScale = Vector3.one;
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
                var tr = _healthPoles[i].transform;
                tr.DOScale(Vector3.zero, _scaleTime).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    tr.gameObject.SetActive(false);  
                });
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
            for (var i = _current; i < totalHealth; i++)
            {
                _healthPoles[i].transform.localScale = Vector3.one;
                _healthPoles[i].SetActive(true);
                _healthPoles[i].transform.DOPunchScale(Vector3.one * .2f, .25f);
            }
            _current = totalHealth;
        }
    }
}