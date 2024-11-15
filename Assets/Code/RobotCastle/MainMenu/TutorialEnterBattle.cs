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
        [SerializeField] private Vector3 _handPointOffset;
        [SerializeField] private Vector3 _handPointOffset2;
        [SerializeField] private Vector3 _handPointOffset3;
        [SerializeField] private Image _background;
        private SubParent _subParent = new();
        private MyButton _btn;
        private bool _isWaiting;
        
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
            _textPrinter.Hide();
            yield return null;
            _btn.RemoveMainCallback(OnPlayBtn);
            var ui = ServiceLocator.Get<IUIManager>().Show<GameModeSelectionUI>(UIConstants.UIGameModeSelection, () => { });
            ui.Show();
            ui.btnChapters2.SetInteractable(false);
            ui.closeBtn.SetInteractable(false);
            var btn = ui.btnChapters1;
            
            _hand.On();
            var pos = btn.transform.position + _handPointOffset2;
            _hand.MoveTo(pos);
            _hand.LoopClicking(pos);
            
            btn.AddMainCallback(GameModeSelected);
            _isWaiting = true;
            while(_isWaiting)
                yield return null;
            _hand.Off();
            yield return null;
            btn.RemoveMainCallback(GameModeSelected);

            var chapterUI = ui.ChapterSelectionUI;
            chapterUI.DisableInputButPlayButton();
            chapterUI.PlayBtn.SetInteractable(true);
            chapterUI.HideAdditionalReward();
            _hand.On();
            _hand.LoopClicking(chapterUI.PlayBtn.transform.position + _handPointOffset3);
        }

        private void GameModeSelected()
        {
            _isWaiting = false;
        }
    }
}