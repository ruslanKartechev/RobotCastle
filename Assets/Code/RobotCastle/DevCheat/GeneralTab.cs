using System;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public class GeneralTab : Tab
    {
        public override void Show(Action closeCallback)
        {
            _closeCallback = closeCallback;
            gameObject.SetActive(true);
            var gm = ServiceLocator.Get<GameMoney>();
            _softMoneyBtn.source = gm.globalMoney;
            _hardMoneyBtn.source = gm.globalHardMoney;
            _returnBtn.AddMainCallback(Return);
            _btnAddLevel.AddMainCallback(AddLevel);
            var lvl = DataHelpers.GetPlayerData().playerLevel;
            _castleLevelText.text = $"Level: {lvl + 1}";
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }


        [SerializeField] private MoneyAddBtn _softMoneyBtn;
        [SerializeField] private MoneyAddBtn _hardMoneyBtn;
        [SerializeField] private MyButton _btnAddLevel;
        [SerializeField] private TextMeshProUGUI _castleLevelText;
        [SerializeField] private MyButton _returnBtn;
        private Action _closeCallback;
        
        private void AddLevel()
        {
            var save = DataHelpers.GetPlayerData();
            var lvl = save.playerLevel;
            lvl++;
            save.playerLevel = lvl;
            _castleLevelText.text = $"Level: {lvl + 1}";
        }
        
        private void Return()
        {
            Close();
            _closeCallback?.Invoke();
        }
    }
}