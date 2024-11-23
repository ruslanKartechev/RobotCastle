using RobotCastle.Core;
using RobotCastle.MainMenu;
using SleepDev.UIElements;
using UnityEngine;

namespace RobotCastle.UI
{
    public class PlayerDataPanelUI : MonoBehaviour, IScreenUI
    {
        public RectTransform moneyPoint => _moneyPoint;

        public RectTransform hardMoneyPoint => _hardMoneyPoint;

        public RectTransform energyPoint => _energyPoint;

        [SerializeField] private RectTransform _moneyPoint;
        [SerializeField] private RectTransform _hardMoneyPoint;
        [SerializeField] private RectTransform _energyPoint;
        
        [SerializeField] private MoneyUI _globalMoney;
        [SerializeField] private MoneyUI _hardMoney;
        [SerializeField] private PlayerExperienceUI _experience;
        [SerializeField] private ValueCurrentAndMaxUI _energy;

        private void OnEnable()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            _globalMoney.Init(gm.globalMoney);
            _globalMoney.DoReact(true);
            _hardMoney.Init(gm.globalHardMoney);
            _hardMoney.DoReact(true);
            
            var xp = ServiceLocator.Get<CastleXpManager>();
            
            _experience.SetPlayerXp(xp.GetLevel(), xp.GetProgressToNextLvl());

            var energy = ServiceLocator.Get<PlayerEnergyManager>();
            _energy.SetMax(energy.GetMax());
            _energy.SetValue(energy.GetCurrent(), 1);
            energy.OnEnergySet += _energy.SetValue;
        }

        private void OnDisable()
        {
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