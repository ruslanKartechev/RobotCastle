using System.Collections.Generic;
using RobotCastle.InvasionMode;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.DevCheat
{
    public class ChapterUI : MonoBehaviour
    {
        public void Init(SaveInvasionProgression.ChapterData chapterData, 
            Chapter chapter, Sprite icon)
        {
            _iconImage.sprite = icon;
            _chapterText.text = chapter.viewName;
            _chapterData = chapterData;
            for (var i = 0; i < chapterData.tierData.Count && i < _tierUI.Count; i++)
            {
                _tierUI[i].tierData = chapterData.tierData[i];
            }
        }
        
        public void UnlockAll()
        {
            _chapterData.unlocked = true;
            foreach (var tier in _tierUI)
            {
                tier.SetUnlocked();
            }
        }
        
        [SerializeField] private List<TierUI> _tierUI;
        [SerializeField] private TextMeshProUGUI _chapterText;
        [SerializeField] private Image _iconImage;
        private SaveInvasionProgression.ChapterData _chapterData;

        [System.Serializable]
        public class TierUI
        {
            public ButtonWithCheck unlockedBtn;
            public ButtonWithCheck passedBtn;
            private bool _didInit;

            public SaveInvasionProgression.TierData tierData
            {
                get => _tierData;
                set
                {
                    _tierData = value;
                    unlockedBtn.SetState(_tierData.unlocked);
                    passedBtn.SetState(_tierData.completed);
                    Init();
                }
            }

            public void SetUnlocked()
            {
                if (_tierData != null)
                {
                    _tierData.unlocked = true;
                    unlockedBtn.SetState(true);
                }
            }

            private void Init()
            {
                if (_didInit)
                    return;
                _didInit = true;
                passedBtn.AddMainCallback(ChangePassedState);
                unlockedBtn.AddMainCallback(ChangeLockedState);
            }

            private void ChangePassedState()
            {
                if (_tierData == null)
                {
                    CLog.LogError("tier data is null");
                    return;
                }

                var state = !_tierData.completed;
                _tierData.completed = state;
                passedBtn.SetState(state);
            }
            
            private void ChangeLockedState()
            {
                if (_tierData == null)
                {
                    CLog.LogError("tier data is null");
                    return;
                }

                var state = !_tierData.unlocked;
                _tierData.unlocked = state;
                unlockedBtn.SetState(state);
            }
            
            private SaveInvasionProgression.TierData _tierData;
        }
    }
}