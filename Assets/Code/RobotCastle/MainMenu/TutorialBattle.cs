using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.MainMenu
{
    public class TutorialBattle : TutorialBase
    {
    
        public override void Begin(Action finishedCallback)
        {
            CLog.LogGreen($"[TutorialBattle] Begin");
            _finishedCallback = finishedCallback;
            _textPrinter.Off();
            SetInput(false);
            StartCoroutine(Working());
        }

        [SerializeField] private float _delayStart = 1f;
        [SerializeField] private int _purchasesCountToStart = 2;
        [SerializeField] private List<string> _messages1;
        [SerializeField] private Vector3 _handPointOffset;
        [SerializeField] private Vector3 _handPointOffsetTroopSize;
        [SerializeField] private Vector3 _handPointOffsetPlay1;
        [SerializeField] private Vector3 _handPointOffsetPlay2;
        [SerializeField] private Image _background;
        [Space(10)]
        [SerializeField] private float _moveTo1 = 1f;
        [SerializeField] private float _moveTo2 = .2f;
        [SerializeField] private float _moveDelay = .5f;
        [SerializeField] private float _playTutordelay = 1f;
        [SerializeField] private int _roundsCount = 5;

        private SubParent _subParent = new();
        private MyButton _btnPurchase;
        private bool _isWaiting;
        private int _purchasesCount = 0;
        
        private bool _didMerge;
        private IItemView _it1;
        private IItemView _it2;
        
        private void SetInput(bool interactable)
        {
            var ui = ServiceLocator.Get<IUIManager>().Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            ui.BtnStart.SetInteractable(interactable);
            ui.BtnStart2.SetInteractable(interactable);
            ui.TroopSizePurchaseUI.SetInteractable(interactable);
            ui.BtnPurchaseHero.SetInteractable(interactable);
        }
 
        private IEnumerator Working()
        {
            ServiceLocator.Get<BattleCamera>().AllowPlayerInput(false);

            _textPrinter.Callback = StopWaiting;
            _textPrinter.Hide();
            _hand.Off();
            _background.enabled = false;
            SetInput(false);
            yield return new WaitForSeconds(_delayStart);

            _panelAnimator.FadeIn();
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages1);
            
            // wait until tutor text played
            _isWaiting = true;
            while (_isWaiting)
                yield return null;

            // Battle merge
            var ui = ServiceLocator.Get<IUIManager>().Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            _btnPurchase = ui.BtnPurchaseHero;
            ShowPurchaseFinger();
            while (_purchasesCount < 1 )
                yield return null;
            
            _textPrinter.Hide();
            while (_purchasesCount < _purchasesCountToStart)
                yield return null;
            _btnPurchase.RemoveMainCallback(OnPurchaseMade);

            if (CanStartMergeTutor())
            {
                yield return MergeTutoring(_it1, _it2);
                yield return null;
            }

            _hand.On();
            var startBtn = (MyButton)ui.BtnStart;
            _hand.MoveToAndLoopClicking(startBtn.transform.position + _handPointOffsetPlay1);
            startBtn.SetInteractable(true);
            startBtn.AddMainCallback(StopWaiting);
            
            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            
            startBtn.RemoveMainCallback(StopWaiting);
            _hand.StopAllActions();
            _hand.Off();
            
            var troopsSizeManager = ServiceLocator.Get<ITroopSizeManager>();
            var playerFactory = ServiceLocator.Get<IPlayerFactory>();
            var money = ServiceLocator.Get<GameMoney>();
            var bm = ServiceLocator.Get<BattleManager>();
            var merge = ServiceLocator.Get<MergeManager>();
            
            
            var rounds = 0;
            while (rounds <= _roundsCount)
            {
                if (bm.battle.State == BattleState.Going)
                    yield return null;
                else
                {
                    if (merge.Container.heroes.Count > 3 
                        && troopsSizeManager.NextCost.Val <= money.levelMoney.Val)
                    {
                        yield return TroopSizeTutoring();
                        yield return null;
                        continue;
                    }
                    if (playerFactory.NextCost.Val <= money.levelMoney.Val)
                    {
                        yield return PurchaseTutoring();
                        yield return null;
                        continue;
                    }
                    if (CanStartMergeTutor())
                    {
                        yield return MergeTutoring(_it1, _it2);
                        yield return null;
                        continue;
                    }
                    if (bm.battle.State != BattleState.Going)
                        SetInput(true);
                    yield return new WaitForSeconds(_playTutordelay);
                    if (bm.battle.State != BattleState.Going)
                    {
                        yield return PlayButtonTutoring();
                        rounds++;
                        CLog.LogYellow($"Next round ======= {rounds}");
                    }
                    else
                    {
                        rounds++;
                        CLog.LogYellow($"Next round ======= {rounds}");
                    }
                }
                yield return null;
            }
            
            CLog.LogBlue($"========== COMPLETED");
            yield return null;
            Complete();
        }
        
        private IEnumerator TroopSizeTutoring()
        {
            CLog.Log($"[{nameof(TutorialBattle)}] TroopSizeTutoring");
            var ui = ServiceLocator.Get<IUIManager>().Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            SetInput(false);
            _hand.On();
            var btn = ui.TroopSizePurchaseUI;
            btn.SetInteractable(true);
            btn.AddMainCallback(StopWaiting);
            
            _hand.LoopClickingTracking(btn.transform, _handPointOffsetTroopSize);
            btn.transform.DOPunchScale(Vector3.one * .12f, .25f);
               
            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            btn.RemoveMainCallback(StopWaiting);
            _hand.Off();
        }

        private IEnumerator PurchaseTutoring()
        {
            CLog.Log($"[{nameof(TutorialBattle)}] PurchaseTutoring");
            SetInput(false);
            ShowPurchaseFinger();
            _isWaiting = true;
            _btnPurchase.SetInteractable(true);
            _btnPurchase.AddMainCallback(StopWaiting);
            while (_isWaiting)
                yield return null;
            _btnPurchase.RemoveMainCallback(StopWaiting);
            _hand.Off();
        }
             
        private IEnumerator MergeTutoring(IItemView it1, IItemView it2)
        {
            CLog.Log($"[{nameof(TutorialBattle)}] MergeTutoring");
            SetInput(false);
            _didMerge = false;
            MoveHandForMerge();
            var m = ServiceLocator.Get<MergeManager>();
            m.MergeController.OnPutItem += OnMergeItemPut;
            while (!_didMerge)
            {
                if (!it1.Transform.gameObject.activeInHierarchy
                    || !it2.Transform.gameObject.activeInHierarchy)
                {
                    _it1 = it1;
                    _it2 = it2;
                    if (it2.itemData.pivotY >= 2)
                    {
                        _it1 = it2;
                        _it2 = it1;
                    }
                    _didMerge = true;
                }
                yield return null;
            }
            m.MergeController.OnPutItem -= OnMergeItemPut;
            _hand.Off();
        }

        private IEnumerator PlayButtonTutoring()
        {
            var cam = ServiceLocator.Get<BattleCamera>();
            var ui = ServiceLocator.Get<IUIManager>().Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            var btn1 = ui.BtnStart;
            var btn2 = ui.BtnStart2;
            btn1.AddMainCallback(StopWaiting);
            btn2.AddMainCallback(StopWaiting);
            _isWaiting = true;
            _hand.On();
            var posIndex = cam.PositionIndex;
            SetHand(posIndex);
            
            while (_isWaiting)
            {
                if (cam.PositionIndex != posIndex)
                {
                    posIndex = cam.PositionIndex;
                    SetHand(posIndex);
                }
                yield return null;
            }
            btn1.RemoveMainCallback(StopWaiting);
            btn2.RemoveMainCallback(StopWaiting);
            _hand.Off();
            
            void SetHand(int index)
            {
                var b = (MyButton)(index == 0 ? btn1 : btn2);
                var offset = index == 0 ? _handPointOffsetPlay1 : _handPointOffsetPlay2;
                _hand.LoopClickingTracking(b.transform, offset);
            }
        }
        
        private void ShowPurchaseFinger()
        {
            _btnPurchase.SetInteractable(true);
            _hand.On();
            _hand.LoopClickingTracking(_btnPurchase.transform, _handPointOffset);
            _btnPurchase.transform.DOPunchScale(Vector3.one * .12f, .25f);   
            _btnPurchase.AddMainCallback(OnPurchaseMade);
        }

        private bool CanStartMergeTutor()
        {
            var bm = ServiceLocator.Get<BattleManager>();
            var m = ServiceLocator.Get<MergeManager>();
            if (bm.battle.State != BattleState.Going)
            {
                foreach (var h1 in m.Container.allItems)
                {
                    foreach (var h2 in m.Container.allItems)
                    {
                        if (h1 == h2) continue;
                        if (h1.itemData.core == h2.itemData.core)
                        {
                            _it1 = h1;
                            _it2 = h2;
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        private void OnMergeItemPut(MergePutResult result)
        {
            CLog.Log($"Result: {result.ToString()}");
            if (result == MergePutResult.Merged)
                _didMerge = true;
            else
                MoveHandForMerge();
        }

        private void MoveHandForMerge()
        {
            var cam = Camera.main;
            _hand.On();
            var p1 = cam.WorldToScreenPoint(_it1.Transform.position);
            var p2 = cam.WorldToScreenPoint(_it2.Transform.position);
            _hand.MoveBetween(p1, p2, _moveTo2, _moveTo1, _moveDelay);
        }
        
        private void Complete()
        {
            SetInput(true);
            ServiceLocator.Get<BattleCamera>().AllowPlayerInput(true);
        }
        
        private void StopWaiting() => _isWaiting = false;

        private void OnPurchaseMade()
        {
            _purchasesCount++;
        }
    }
}