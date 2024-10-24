using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public class ChaptersTab : Tab
    {

        public void InitChapters()
        {
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            var db = ServiceLocator.Get<ProgressionDataBase>();
            var save = DataHelpers.GetPlayerData().progression;
            for (var i = 0; i < _chapters.Count && i < save.chapters.Count; i++)
            {
                var icon = Resources.Load<Sprite>(viewDb.LocationIcons[i]);
                
                _chapters[i].Init(save.chapters[i], db.chapters[i], icon);
            }
        }
        
        
        public override void Show(Action closeCallback)
        {
            _returnCallback = closeCallback;
            gameObject.SetActive(true);
            InitChapters();
            _btnReturn.AddMainCallback(Return);
            _btnUnlockAll.AddMainCallback(UnlockAll);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
        
        [SerializeField] private List<ChapterUI> _chapters;
        [SerializeField] private MyButton _btnUnlockAll;
        [SerializeField] private MyButton _btnReturn;
        private Action _returnCallback;

        private void Return()
        {
            Close();
            _returnCallback?.Invoke();
        }
        
        private void UnlockAll()
        {
            foreach (var ch in _chapters)
                ch.UnlockAll();
        }

    }
}