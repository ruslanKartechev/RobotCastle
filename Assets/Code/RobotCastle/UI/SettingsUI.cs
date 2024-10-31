using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class SettingsUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private SwitchButton _btnSound;
        [SerializeField] private SwitchButton _btnVibr;
        [SerializeField] private MyButton _btnClose;
        [SerializeField] private Button _btnPrivacy;
        [SerializeField] private List<Button> _buttons;
        [SerializeField] private FadeInOutAnimator _fadeInOutAnimator;
        [SerializeField] private string _link;


        public void Show()
        {
            _btnPrivacy.onClick.RemoveListener(OnPrivacy);
            _btnPrivacy.onClick.AddListener(OnPrivacy);
            _btnClose.AddMainCallback(Close);
            foreach (var btn in _buttons)
                btn.interactable = true;

            var data = DataHelpers.GetPlayerData();
            if(data.soundOn)
                _btnSound.SetActive(false);
            else
                _btnSound.SetPassive(false);
            
            if(data.vibrOn)
                _btnVibr.SetActive(false);
            else
                _btnVibr.SetPassive(false);
            
            _btnSound.AddMainCallback(SwitchSound);
            _btnVibr.AddMainCallback(SwitchVibr);
            
            _fadeInOutAnimator.On();
            _fadeInOutAnimator.FadeIn();
        }

        private void SwitchVibr()
        {
            var data = DataHelpers.GetPlayerData();
            var dd = !data.vibrOn;
            if(dd)
                _btnVibr.SetActive(true);
            else
                _btnVibr.SetPassive(true);
            data.vibrOn = dd;            
        }

        private void SwitchSound()
        {
            var data = DataHelpers.GetPlayerData();
            var dd = !data.soundOn;
            if(dd)
                _btnSound.SetActive(true);
            else
                _btnSound.SetPassive(true);
            data.soundOn = dd;
            data.musicOn = dd;
            SoundManager.Inst.SetStatusSound(dd);
            SoundManager.Inst.SetStatusMusic(dd);
        }

        private void Close()
        {
            foreach (var btn in _buttons)
                btn.interactable = false;
            
            _fadeInOutAnimator.FadeOut();
            ServiceLocator.Get<IUIManager>().OnClosed(UIConstants.UISettings);
        }
        
        private void OnPrivacy()
        {
            Application.OpenURL(_link);
        }
        
    }
}