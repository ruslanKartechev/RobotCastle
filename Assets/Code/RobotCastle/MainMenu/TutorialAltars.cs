using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using RobotCastle.Battling.Altars;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.MainMenu
{
    public class TutorialAltars : TutorialBase
    {
        public override string Id => "altars";
        
        public override void Begin(Action finishedCallback)
        {
            CLog.Log($"[TutorialBattle] Begin");
            _finishedCallback = finishedCallback;
            var altars = ServiceLocator.Get<AltarManager>();
            var cost = altars.GetPointCost();
            ServiceLocator.Get<GameMoney>().AddGlobalMoney(cost);
            StartCoroutine(Working());
        }

        [SerializeField] private List<string> _messages1;
        [SerializeField] private List<string> _messages2;
        [Space(10)]
        [SerializeField] private Vector3 _clickOffsetBarracks;
        [SerializeField] private Vector3 _btnSummonOffset;
        [SerializeField] private Vector3 _btnBuyPointOffset;
        [SerializeField] private Vector3 _altarUpgradeOffset;
        [SerializeField] private Vector3 _btnCloseOffset;
        [Space(10)]
        [SerializeField] private Image _background;
        [SerializeField] private float _handMoveTime = .6f;
        [Space(10)]
        [SerializeField] private float _btnPunchScale = .1f;
        [SerializeField] private float _btnPunchTime = .25f;
        private SubParent _subParent = new();

        private IEnumerator Working()
        {
            _hand.Off();
            _background.enabled = true;
            var ui = ServiceLocator.Get<IUIManager>();
            var tabs = ui.GetIfShown<MainMenuTabsUI>(UIConstants.UIMainMenuTabs);
            tabs.SetAllInteractable(false);
            _subParent.Parent(tabs.ButtonsParent, _parent);
            _textPrinter.Callback = StopWaiting;
            var barracksBtn = tabs.barracksBtn;
            barracksBtn.SetInteractable(false);
            _textPrinter.Callback = StopWaiting;
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages1);

            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            _textPrinter.Callback = () => {};
          
            barracksBtn.SetInteractable(true);
            _hand.On();
            _hand.LoopClickingTracking(barracksBtn.transform.GetChild(0), _clickOffsetBarracks, 0f);
            yield return WaitForBtn(barracksBtn);
            _subParent.Return();
            _background.enabled = false;
            _textPrinter.Hide();
            
            var barracks = ui.GetIfShown<BarracksTabUI>(UIConstants.UIBarracksTab);
            barracks.altarsBtn.SetInteractable(true);
            barracks.summonBtn.SetInteractable(false);
            var pos = barracks.altarsBtn.transform.position + _btnSummonOffset;
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            var barracksManager = ServiceLocator.Get<BarracksManager>();
            barracksManager.BarracksInput.enabled = false;
       
            yield return WaitForBtn(barracks.altarsBtn);
            _hand.Off();
            barracks.summonBtn.SetInteractable(true);
            yield return null;
            _background.enabled = true;
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages2);
            _textPrinter.Callback = StopWaiting;
            
            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            _background.enabled = false;
            _textPrinter.Hide();
            
            var altars = ui.GetIfShown<AltarsOverviewUI>(UIConstants.UIAltars);
            altars.BtnClose.SetInteractable(false);
            pos = altars.BtnPurchaseNewPoint.transform.position + _btnBuyPointOffset;
            _hand.On();
            _hand.LoopClicking(pos);
            yield return WaitForBtn(altars.BtnPurchaseNewPoint);
            foreach (var alt in altars.Altars)
            {
                var tr = alt.LvlUpBtnOne.transform;
                tr.DOScale(Vector3.one * _btnPunchScale, _btnPunchTime)
                    .OnComplete(() => { tr.DOScale(Vector3.one , _btnPunchTime); });
            }
            
            var btn = altars.Altars[0].LvlUpBtnOne;
            pos = btn.transform.position + _altarUpgradeOffset;
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            
            var manager = ServiceLocator.Get<AltarManager>();
            manager.OnFreePointsCountChanged += OnPointsChanged;
            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            manager.OnFreePointsCountChanged -= OnPointsChanged;


            pos = altars.BtnClose.transform.position + _btnCloseOffset;
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            altars.BtnClose.SetInteractable(true);
            yield return WaitForBtn(altars.BtnClose);
            
            tabs.SetAllInteractable(true);
            _finishedCallback?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnPointsChanged(int prevvalue, int newvalue)
        {
            if (newvalue < prevvalue)
                _isWaiting = false;
        }
    }
}