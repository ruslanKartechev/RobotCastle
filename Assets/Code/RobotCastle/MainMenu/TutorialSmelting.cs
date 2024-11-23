using System;
using System.Collections;
using System.Collections.Generic;
using RobotCastle.Battling.SmeltingOffer;
using SleepDev.Inventory;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class TutorialSmelting : TutorialBase
    {
        public override string Id => "smelting";

        public void SetSmeltingUI(SmeltingOfferUI smeltingUI)
        {
            _smeltingUI = smeltingUI;
        }
    
        public override void Begin(Action finishedCallback)
        {
            _finishedCallback = finishedCallback;
            StartCoroutine(Working());
        }

        public void Stop()
        {
            _hand.Off();
            gameObject.SetActive(false);
        }

        [SerializeField] private int _smeltingIndex = 1;
        [SerializeField] private Vector3 _clickOffset;
        [SerializeField] private Vector3 _clickOffsetBtn;
        [SerializeField] private List<string> _messages;
        private SmeltingOfferUI _smeltingUI;
        private Coroutine _working;

        private void OnDisable()
        {
            _hand.Off();
        }
        
        private IEnumerator Working()
        {
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages);
            _textPrinter.Callback = StopWaiting; 
            _hand.Off();
            yield return null;
            yield return null;
            
            var item = _smeltingUI.ItemsUI[_smeltingIndex];
            _smeltingUI.Inventory.OnNewPicked += OnItemPick;
            _hand.On();
            _hand.LoopClicking(item.transform.position + _clickOffset);
            
            _isWaiting = true;
            while(_isWaiting)
                yield return null;
            
            _textPrinter.Hide();
            _smeltingUI.Inventory.OnNewPicked -= OnItemPick;
            _hand.LoopClicking(_smeltingUI.ConfirmButton.transform.position + _clickOffsetBtn);
            _finishedCallback.Invoke();
        }

        private void OnItemPick(Item obj) => _isWaiting = false;

        private void StopWaiting() {}
        
    }
}