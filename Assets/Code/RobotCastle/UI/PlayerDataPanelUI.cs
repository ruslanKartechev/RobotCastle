using RobotCastle.Core;
using RobotCastle.MainMenu;
using UnityEngine;

namespace RobotCastle.UI
{
    public class PlayerDataPanelUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private StatValueUI _globalMoney;
        [SerializeField] private StatValueUI _hardMoney;
        [SerializeField] private PlayerExperienceUI _experience;
        [SerializeField] private ValueCurrentAndMaxUI _energy;
        

        private void OnEnable()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            _globalMoney.Set(gm.globalMoney);
            _hardMoney.Set(gm.globalHardMoney);
            gm.OnGlobalMoneySet += _globalMoney.OnNewValueSet;
            gm.OnGlobalHardMoneySet += _hardMoney.OnNewValueSet;
            
            var xp = ServiceLocator.Get<CastleXpManager>();
            _experience.SetPlayerXp(xp.GetLevel(), xp.GetProgressToNextLvl());

            var energy = ServiceLocator.Get<PlayerEnergyManager>();
            _energy.SetValue(energy.GetCurrent(), energy.GetMax());
            energy.OnEnergySet += _energy.SetValue;

        }

        private void OnDisable()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            gm.OnGlobalMoneySet -= _globalMoney.OnNewValueSet;
            gm.OnGlobalHardMoneySet -= _hardMoney.OnNewValueSet;
            
            var energy = ServiceLocator.Get<PlayerEnergyManager>();
            energy.OnEnergySet -= _energy.SetValue;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}