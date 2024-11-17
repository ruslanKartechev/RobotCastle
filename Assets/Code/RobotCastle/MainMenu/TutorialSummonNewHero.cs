using System;
using System.Collections;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.MainMenu
{
    public class TutorialSummonNewHero : TutorialBase
    {
        public override void Begin(Action finishedCallback)
        {
            CLog.LogGreen($"[TutorialBattle] Begin");
            _finishedCallback = finishedCallback;
            StartCoroutine(Working());
        }

        [SerializeField] private List<string> _messages1;
        [SerializeField] private List<string> _messages2;
        [SerializeField] private Vector3 _btnSummonOffset;
        [SerializeField] private Vector3 _clickOffsetBarracks;
        [SerializeField] private float _hideDelaySec = 1f;
        [SerializeField] private Image _background;
        [SerializeField] private float _handMoveTime = .6f;
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
            _hand.LoopClickingTracking(barracksBtn.transform, _clickOffsetBarracks, 0f);
            yield return WaitForBtn(barracksBtn);
            _subParent.Return();
            _background.enabled = false;
            _textPrinter.Hide();

            var barracks = ui.GetIfShown<BarracksTabUI>(UIConstants.UIBarracksTab);
            barracks.altarsBtn.SetInteractable(false);
            barracks.summonBtn.SetInteractable(true);
            var pos = barracks.summonBtn.transform.position + _btnSummonOffset;
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            var barracksManager = ServiceLocator.Get<BarracksManager>();
            barracksManager.BarracksInput.SetActive(false);
       
            yield return WaitForBtn(barracks.summonBtn);
     
            _hand.Off();
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages2);
            _textPrinter.Callback = StopWaiting;
            _background.enabled = true;
            
            _isWaiting = true;
            while(_isWaiting)
                yield return null;
            yield return new WaitForSeconds(_hideDelaySec);
            tabs.SetAllInteractable(true);
            Complete();
        }
        
        private void Complete()
        {
            _panelAnimator.FadeOut();
            var barracksManager = ServiceLocator.Get<BarracksManager>();
            barracksManager.BarracksInput.SetActive(true);
            var tabs = ServiceLocator.Get<IUIManager>().GetIfShown<MainMenuTabsUI>(UIConstants.UIMainMenuTabs);
            if(tabs != null)
                tabs.SetAllInteractable(true);
            _finishedCallback?.Invoke();
        }
        
        
    }
}