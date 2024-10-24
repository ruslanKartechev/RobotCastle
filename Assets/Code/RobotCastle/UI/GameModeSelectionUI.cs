﻿using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.UI
{
    public class GameModeSelectionUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private MyButton _btnChapters1;
        [SerializeField] private MyButton _btnChapters2;
        [SerializeField] private MyButton _closeBtn;
        [SerializeField] private Canvas _selectionCanvas;
        [SerializeField] private List<Canvas> _otherCanvases;
        [Space(5)]
        [SerializeField] private InvasionChapterSelectionUI _chapters1;
        [SerializeField] private InvasionChapterSelectionUI _chapters2;
        [SerializeField] private BlackoutFadeScreen _fadeScreen;
        

        public void Show()
        {
            gameObject.SetActive(true);
            foreach (var canvas in _otherCanvases)
                canvas.enabled = false;
            _chapters1.Off();
            _chapters2.Off();
            _selectionCanvas.enabled = true;
            _btnChapters1.AddMainCallback(ShowChapter1);
            _btnChapters2.AddMainCallback(ShowChapter2);
            _closeBtn.AddMainCallback(Return);
            _fadeScreen.FadeInWithId(UIConstants.UIGameModeSelection);
        }

        private void Return()
        {
            _fadeScreen.FadeOut();
        }

        private void ShowChapter1()
        {
            _selectionCanvas.enabled = false;
            _chapters1.Show(ModeReturnCallback);
        }

        private void ShowChapter2()
        {
            _selectionCanvas.enabled = false;
            _chapters2.Show(ModeReturnCallback);
        }

        public void ModeReturnCallback()
        {
            _selectionCanvas.enabled = true;
        }
    }
}