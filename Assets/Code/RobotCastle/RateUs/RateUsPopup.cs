using System;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace MergeHunt
{
    public class RateUsPopup : MonoBehaviour, IScreenUI
    {
        [SerializeField] private RateUsManager _rateUs;
        [SerializeField] private PopAnimator _animator;
        [SerializeField] private Button _skipBtn;
        [SerializeField] private Button _rateBtn;
        private Action _callback;
        
        public void Show(Action callback)
        {
            _callback = callback;
            On();
            _animator.ZeroAndPlay(OnAnimatedOpen);
        }
        

        public void On()
        {
            gameObject.SetActive(true);
        }
        
        public void Off() => gameObject.SetActive(false);

        private void OnAnimatedOpen()
        {
            _rateBtn.interactable = true;
            _skipBtn.interactable = true;
        }

        private void OnBtnSkip()
        {
            _rateUs.Reject();
            OffAndCallback();
        }

        private void OnBtnRate()
        {
            _rateUs.Accept(() =>
            {
                _animator.PlayBackwards(OffAndCallback);
            });
        }

        private void OffAndCallback()
        {
            Off();
            _callback.Invoke();
        }
        
        private void OnEnable()
        {
            _rateBtn.onClick.AddListener(OnBtnRate);
            _skipBtn.onClick.AddListener(OnBtnSkip);
        }

        private void OnDisable()
        {
            _rateBtn.onClick.RemoveListener(OnBtnRate);
            _skipBtn.onClick.RemoveListener(OnBtnSkip);
        }
    }
}