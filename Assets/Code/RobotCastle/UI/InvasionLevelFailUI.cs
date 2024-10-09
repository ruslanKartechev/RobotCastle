using System;
using System.Collections;
using RobotCastle.Core;
using RobotCastle.MainMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class InvasionLevelFailUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private MyButton _btnPlayAgain;
        [SerializeField] private MyButton _btnReturn;
        [SerializeField] private float _buttonsAnimationDelay = 0.3f;
        [SerializeField] private FadeInOutAnimator _fadeAnimatorButtons;
        [SerializeField] private TextMeshProUGUI _lvlText;
        [SerializeField] private Image _xpImage;
        private Action _replay;
        private Action _return;
        private bool _inputActive;
        
        public void Show(Action callbackReplay, Action callbackReturn)
        {
            _return = callbackReturn;
            _replay = callbackReplay;
            _btnPlayAgain.AddMainCallback(Replay);
            _btnReturn.AddMainCallback(Return);
            // var playerData = DataHelpers.GetPlayerData();
            var xp = ServiceLocator.Get<CastleXpManager>();
            _xpImage.fillAmount = xp.GetProgressToNextLvl();
            _lvlText.text = $"{xp.GetLevel() + 1}";
            StartCoroutine(Animating());
            _inputActive = true;
            _btnReturn.AddMainCallback(Return);
            _btnPlayAgain.AddMainCallback(Replay);
        }

        private void Replay()
        {
            if (!_inputActive) return;
            _inputActive = false;
            _replay?.Invoke();
        }

        private void Return()
        {
            if (!_inputActive) return;
            _inputActive = false;
            _return?.Invoke();
        }

        private IEnumerator Animating()
        {
            _fadeAnimatorButtons.Off();
            yield return new WaitForSeconds(_buttonsAnimationDelay);
            _fadeAnimatorButtons.On();
            _fadeAnimatorButtons.FadeIn();
        }
    }
}