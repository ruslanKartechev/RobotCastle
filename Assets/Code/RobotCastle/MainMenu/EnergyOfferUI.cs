using System;
using RobotCastle.UI;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class EnergyOfferUI : MonoBehaviour, IScreenUI
    {
        public void Show(int amount, Action<bool> callback)
        {
            gameObject.SetActive(true);
            _amountText.text = $"+{amount}";
            _callback = callback;
            _btnAccept.AddMainCallback(AcceptOffer);
            _btnRefuse.AddMainCallback(Close);
            _animator.ZeroAndPlay();
        }

        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private MyButton _btnAccept;
        [SerializeField] private MyButton _btnRefuse;
        [SerializeField] private PopAnimator _animator;
        private Action<bool> _callback;

        private void AcceptOffer()
        {
            gameObject.SetActive(false);
            _callback?.Invoke(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
            _callback?.Invoke(false);
        }
    }
}