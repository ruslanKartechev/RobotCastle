using System;
using System.Collections.Generic;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class TutorialWin : TutorialBase
    {
        public override string Id => "win_screen_exit";
    
        public void SetUI(InvasionLevelWinUI ui)
        {
            _ui = ui;
        }
    
        public override void Begin(Action finishedCallback)
        {
            CLog.Log($"[TutorialWin] Begin");
            _finishedCallback = finishedCallback;
            _hand.On();
            var replayBtn = _ui.BtnPlayAgain;
            replayBtn.SetInteractable(false);
            var returnBtn = _ui.BtnReturn;
            _hand.LoopClickingTracking(returnBtn.transform, _clickOffset, 0f);
            returnBtn.SetInteractable(true);
        }
        
        [SerializeField] private List<string> _messages1;
        [SerializeField] private Vector3 _clickOffset;
        private InvasionLevelWinUI _ui;

    }
}