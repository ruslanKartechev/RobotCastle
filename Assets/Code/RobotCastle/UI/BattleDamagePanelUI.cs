using System;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Battling;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BattleDamagePanelUI : MonoBehaviour, IDamageDisplay, IScreenUI
    {
        [SerializeField] private Ease _ease;
        
        [SerializeField] private Pool _physical;
        [SerializeField] private Pool _magical;
        [SerializeField] private Pool _mightyBlock;
        [SerializeField] private Pool _vampirism;

        private void Start()
        {
            _physical.Init();
            _magical.Init();
            _mightyBlock.Init();
            _vampirism.Init();
        }

        public void ShowAt(int amount, EDamageType type, Vector3 worldPosition)
        {
            DamageUI ui = null;
            switch (type)
            {
                case EDamageType.Magical:
                    ui = (DamageUI)_magical.GetOne();
                    ui.pool = _magical;
                    break;
                case EDamageType.Physical:
                    ui = (DamageUI)_physical.GetOne();
                    ui.pool = _physical;
                    break;
            }
            
            ui.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
            ui.Show(amount);
            ui.AnimateDamage(_ease);
        }
        
        public void ShowAtScreenPos(int amount, EDamageType type, Vector3 screenPos)
        {
            DamageUI ui = null;
            switch (type)
            {
                case EDamageType.Magical:
                    ui = (DamageUI)_magical.GetOne();
                    ui.pool = _magical;
                    break;
                case EDamageType.Physical:
                    ui = (DamageUI)_physical.GetOne();
                    ui.pool = _physical;
                    break;
            }

            ui.transform.position = screenPos;
            ui.Show(amount);
            ui.AnimateDamage(_ease);
        }

        public void ShowMightyBlock(Vector3 worldPosition)
        {
            var ui = (DamageUI)_mightyBlock.GetOne();
            ui.pool = _mightyBlock;
            ui.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
            ui.ShowMightyBlock();
        }

        public void ShowVampirism(int amount, Vector3 worldPosition)
        {
            var ui = (DamageUI)_vampirism.GetOne();
            ui.pool = _vampirism;
            ui.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
            ui.ShowVampirism(amount);
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