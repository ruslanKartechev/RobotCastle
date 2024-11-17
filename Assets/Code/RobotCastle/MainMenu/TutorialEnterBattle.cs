using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.MainMenu
{
    public class TutorialEnterBattle : TutorialBase
    {
        public override void Begin(Action finishedCallback)
        {
            CLog.LogGreen($"[TutorialEnterBattle] Begin");
            _finishedCallback = finishedCallback;
            _panelAnimator.FadeIn();
            _textPrinter.Callback = Callback;
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages1);
            _hand.Off();
            _background.enabled = true;
            
            var s = DataHelpers.GetPlayerData();
            s.chapterSelectionData.chapterIndex = 0;
            s.chapterSelectionData.tierIndex = 0;
            s.chapterSelectionData.corruption = false;

        }

        [SerializeField] private List<string> _messages1;
        [SerializeField] private float _handMoveTime = .6f;
        [SerializeField] private Vector3 _handPointOffset;
        [SerializeField] private Vector3 _handPointOffset2;
        [SerializeField] private Vector3 _handPointOffset3;
        [SerializeField] private Image _background;
        private SubParent _subParent = new();
        private MyButton _btn;
        
        private void Callback()
        {
            _hand.On();
            var ui = ServiceLocator.Get<IUIManager>().Show<GateTabUI>(UIConstants.UIGateTab, () => { });
            _btn = ui.battleBtn;
            _btn.SetInteractable(true);
            _subParent.Parent(_btn.transform, _parent);
            _btn.AddMainCallback(OnPlayBtn);
            _hand.LoopClicking(_handPointOffset + _btn.transform.position);
            _btn.transform.DOPunchScale(Vector3.one * .3f, .5f);
        }

        private void OnPlayBtn()
        {
            _subParent.Return();
            _background.enabled = false;
            _textPrinter.Hide();
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            _hand.StopAllActions();
            _hand.Off();
            _textPrinter.Off();
            yield return null;
            _btn.RemoveMainCallback(OnPlayBtn);
            var gameModeUI = ServiceLocator.Get<IUIManager>().Show<GameModeSelectionUI>(UIConstants.UIGameModeSelection, () => { });
            gameModeUI.Show();
            gameModeUI.btnChapters2.SetInteractable(false);
            gameModeUI.closeBtn.SetInteractable(false);
            var chaptersBtn = gameModeUI.btnChapters1;
            
            _hand.On();
            var pos = chaptersBtn.transform.position + _handPointOffset2;
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            yield return WaitForBtn(chaptersBtn);
            _hand.Off();
            yield return null;

            var chapterUI = gameModeUI.ChapterSelectionUI;
            chapterUI.DisableInputButPlayButton();
            chapterUI.PlayBtn.SetInteractable(true);
            chapterUI.HideAdditionalReward();
            _hand.On();
            _hand.MoveToAndLoopClicking(chapterUI.PlayBtn.transform.position + _handPointOffset3, _handMoveTime);
            _finishedCallback?.Invoke();
        }
        
    }
}