using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Battling;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BattleDamagePanelUI : MonoBehaviour, IDamageDisplay, IScreenUI
    {
        [SerializeField] private float _animateTime;
        [SerializeField] private Ease _ease;
        
        [SerializeField] private Data _physical;
        [SerializeField] private Data _magical;
        
        
        public void ShowAt(int amount, EDamageType type, Vector3 worldPosition)
        {
            DamageUI ui = null;
            switch (type)
            {
                case EDamageType.Magical:
                    ui = _magical.GetOne();
                    break;
                case EDamageType.Physical:
                    ui = _physical.GetOne();
                    break;
            }
            
            ui.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
            ui.Show(amount);
            ui.Animate(_ease);
        }

        private void OnEnable()
        {
            ServiceLocator.Bind<IDamageDisplay>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<IDamageDisplay>();
        }

        [System.Serializable]
        private class Data
        {
            [SerializeField] private List<DamageUI> _uis;
            private int _index;

            public DamageUI GetOne()
            {
                var ui = _uis[_index];
                _index++;
                if (_index >= _uis.Count)
                    _index = 0;
                return ui;
            }

        }
    }
}