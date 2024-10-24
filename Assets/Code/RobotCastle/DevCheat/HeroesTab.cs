using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public class HeroesTab : Tab
    {
        public void InitHeroes()
        {
            var db = ServiceLocator.Get<HeroesDatabase>();
            var saves = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            for (var i = 0; i < saves.heroSaves.Count && i < _heroUI.Count; i++)
            {
                var ui = _heroUI[i];
                var save = saves.heroSaves[i];
                var icon = ViewDataBase.GetHeroSprite(db.info[save.id].viewInfo.iconId);
                var name = db.GetHeroViewInfo(save.id).name;
                ui.Init(save, name, icon);                
            }
        }
        
        public override void Show(Action closeCallback)
        {
            _closeCallback = closeCallback;
            _closeBtn.AddMainCallback(Return);
            _unlockAllBtn.AddMainCallback(UnlockAll);
            gameObject.SetActive(true);
            InitHeroes();
        }

        public override void Close()
        {
            gameObject.SetActive(false);        
        }

        [SerializeField] private List<HeroUI> _heroUI;
        [SerializeField] private MyButton _closeBtn;
        [SerializeField] private MyButton _unlockAllBtn;
        private Action _closeCallback;

        private void UnlockAll()
        {
            foreach (var ui in _heroUI)
                ui.Unlock();
        }

        private void Return()
        {
            Close();
            _closeCallback?.Invoke();
        }
    }
}