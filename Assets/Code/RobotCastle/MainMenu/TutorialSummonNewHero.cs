using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Summoning;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.MainMenu
{
    public class TutorialSummonNewHero : TutorialBase
    {
    
        public override string Id => "hero_summon";

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
            yield return null;
            
            _hand.Off();
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages2);
            _textPrinter.Callback = StopWaiting;
            _background.enabled = true;
            
            _isWaiting = true;
            while(_isWaiting)
                yield return null;
            
            _textPrinter.Hide();
            _background.enabled = false;
            var summonUI = ui.GetIfShown<SummoningUI>(UIConstants.UISummon);
            if (summonUI == null)
            {
                CLog.LogError($"Summon UI is null");
            }
            summonUI.Scroller.inputAllowed = false;
            var subButtons = new List<MyButton>(4);
            var op = (SummonOptionUI)summonUI.Scroller.currentObject;
            var clickTarget = (Transform)null;
            if (op.btn1 != null)
            {
                subButtons.Add(op.btn1);
                op.btn1.AddMainCallback(StopWaiting);
                clickTarget = op.btn1.transform;
            }
            if (op.btn2 != null)
            {
                subButtons.Add(op.btn2);
                op.btn2.AddMainCallback(StopWaiting);
                if(clickTarget is null)
                    clickTarget = op.btn2.transform;
                
            }
            if (op.btn3 != null)
            {
                subButtons.Add(op.btn3);
                op.btn3.AddMainCallback(StopWaiting);
                if(clickTarget is null)
                    clickTarget = op.btn3.transform;
            }
            if (op.btnOptional != null)
            {
                subButtons.Add(op.btnOptional);
                op.btnOptional.AddMainCallback(StopWaiting);
                if(clickTarget is null)
                    clickTarget = op.btnOptional.transform;
            }
            _hand.On();
            _hand.LoopClickingTracking(clickTarget, Vector3.zero, 0f);
            _isWaiting = true;
            while(_isWaiting)
                yield return null;
            _hand.Off();

            foreach (var bb in subButtons)
                bb.RemoveMainCallback(StopWaiting);
            summonUI.Scroller.inputAllowed = true;
            Complete();
            // yield return new WaitForSeconds(_hideDelaySec);
            // tabs.SetAllInteractable(true);
            // Complete();
        }
        
        private void Complete()
        {
            _panelAnimator.Off();
            var barracksManager = ServiceLocator.Get<BarracksManager>();
            barracksManager.BarracksInput.SetActive(true);
            var tabs = ServiceLocator.Get<IUIManager>().GetIfShown<MainMenuTabsUI>(UIConstants.UIMainMenuTabs);
            if(tabs != null)
                tabs.SetAllInteractable(true);
            _finishedCallback?.Invoke();
        }
        
        
    }
}