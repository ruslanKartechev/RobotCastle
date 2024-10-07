﻿using UnityEngine;

namespace RobotCastle.UI
{
    public class GameModeSelectionUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private MyButton _btnChapters1;
        [SerializeField] private MyButton _btnChapters2;
        [SerializeField] private Canvas _selectionCanvas;
        [Space(5)]
        [SerializeField] private InvasionChapterSelectionUI _chapters1;
        [SerializeField] private InvasionChapterSelectionUI _chapters2;
        

        public void Show()
        {
            _chapters1.Off();
            _chapters2.Off();
            _selectionCanvas.enabled = true;
            gameObject.SetActive(true);
            _btnChapters1.AddMainCallback(ShowChapter1);
            _btnChapters2.AddMainCallback(ShowChapter2);
        }

        private void ShowChapter1()
        {
            _selectionCanvas.enabled = false;
            _chapters1.Show();
        }

        private void ShowChapter2()
        {
            _selectionCanvas.enabled = false;
            _chapters2.Show();
        }

        public void ModeReturnCallback()
        {
            _selectionCanvas.enabled = true;
        }
    }
}