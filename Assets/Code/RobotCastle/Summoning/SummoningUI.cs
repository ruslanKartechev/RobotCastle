using System;
using System.Collections.Generic;
using System.Threading;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Summoning
{
    // Common scroll - three buttons (x1, x4, x20). If Free available - replaces 1st button 
    // King scroll - three or two buttons (x1, x4). If Ad available - adds the 1st button on the left
    // Gos scroll - 2 buttons (x1, x4), no other options (purchase for 80 and 320)
    // New hero scroll - 2 buttons, summon for scroll and purchase for 100 medals
    public class SummoningUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private List<SummonOptionUI> _options;
        [SerializeField] private LeftRightScroller _scroller;
        [SerializeField] private GameObject _freeLeftBadge;
        [SerializeField] private GameObject _freeRightBadge;
        [SerializeField] private MyButton _returnBtn;
        private SummoningManager _manager = new();
        private CancellationTokenSource _tokenSource;
        private bool _canTakeInput;

        private CardsOutputDisplay _cardsDisplay; 
        
        private CardsOutputDisplay cardsDisplay
        {
            get
            {
                if (_cardsDisplay != null) return _cardsDisplay;
                _cardsDisplay = ServiceLocator.Get<IUIManager>().Show<CardsOutputDisplay>(UIConstants.UISummonCard, () => {});
                return _cardsDisplay;
            }
        }
        
        private SummonOptionUI currentUI => (SummonOptionUI)_scroller.currentObject;
        
        public void Show()
        {
            gameObject.SetActive(true);
            InitData();
            _returnBtn.AddMainCallback(Return);
            _scroller.inputAllowed = true;
            _scroller.assignedObjects.Clear();
            for (var i = 0; i < _options.Count; i++)
            {
                _scroller.assignedObjects.Add(_options[i]);
                AdCallbacks(_options[i]);
            }
            
            _scroller.OnNewItemChosen -= OnNewItemPicked;
            _scroller.OnNewItemChosen += OnNewItemPicked;
            AdCallbacks(currentUI);
            SetFreeBadgesOnScroll();
            _canTakeInput = true;
        }

        private void SetFreeBadgesOnScroll()
        {
            var left = _scroller.nextLeftObject as SummonOptionUI;
            if (left == null)
            {
                CLog.LogError("Left obj is null");
                _freeLeftBadge.gameObject.SetActive(false);
                _freeRightBadge.gameObject.SetActive(false);
                return;
            }
            var right = _scroller.nextRightObject as SummonOptionUI;
            if (right == null)
            {
                CLog.LogError("Right obj is null");
                _freeLeftBadge.gameObject.SetActive(false);
                _freeRightBadge.gameObject.SetActive(false);
                return;
            }
            _freeLeftBadge.gameObject.SetActive(SummoningManager.CanTakeForFreeOrForAd(left.id));
            _freeRightBadge.gameObject.SetActive(SummoningManager.CanTakeForFreeOrForAd(right.id));
        }
        
        private void OnScrollAd()
        {
            if (!_canTakeInput) return;

            CLog.Log($"[{nameof(SummoningUI)}] OnScrollAd. Id: {currentUI.id}. Count: {currentUI.multiplier_1}");
            // var results = SummoningManager.GetAndApplyResults(currentUI.id);
            if (SummoningManager.CanTakeForAds(currentUI.id))
            {
                var placement = $"{GlobalConfig.Rewarded_ScrollForAd}_{currentUI.id}";
                var (can, msg) = SleepDev.AdsPlayer.Instance.PlayReward((bool didPlay) =>
                {
                    if (didPlay)
                    {
                        SummoningManager.SetOffTimerAndAddOpenedScrollCount(currentUI.id);
                        var results = SummoningManager.GetRandomItemsAndApply(currentUI.id);
                        ShowCardsOpening(results, _tokenSource.Token);        
                    }
                    else
                    {
                        CLog.Log("Failed to play ad");
                    }
                }, placement);
                if (!can)
                {
                    CLog.Log($"Cannot play ad. {msg}");
                    return;
                }
            }
        }

        private void OnScrollFree()
        {
            if (!_canTakeInput) return;
            CLog.Log($"[{nameof(SummoningUI)}] OnScrollFree. Id: {currentUI.id}. Count: {currentUI.multiplier_1}");
            if (SummoningManager.CanTakeForFree(currentUI.id))
            {
                SummoningManager.SetOffTimerAndAddOpenedScrollCount(currentUI.id);
                var results = SummoningManager.GetRandomItemsAndApply(currentUI.id);
                ShowCardsOpening(results, _tokenSource.Token);
            }
        }

        private void OnCountX1()
        {
            if(!_canTakeInput) return;
            CLog.Log($"[{nameof(SummoningUI)}] OnCountX1. Id: {currentUI.id}. Count: {currentUI.multiplier_1}");
            var didPurchase = SummoningManager.TryGetOutputFromScroll(currentUI.id, (int)currentUI.multiplier_1, 
                currentUI.multiplier_1, out var results);
            if (didPurchase)
            {
                SummoningManager.AddOpenedScrollCount(currentUI.id);
                ShowCardsOpening(results, _tokenSource.Token);
            }
        }

        private void OnCountX2()
        {
            if(!_canTakeInput) return;
            CLog.Log($"[{nameof(SummoningUI)}] OnCountX2. Id: {currentUI.id}. Count: {currentUI.multiplier_2}");
            var didPurchase = SummoningManager.TryGetOutputFromScroll(currentUI.id, (int)currentUI.multiplier_2, 
                currentUI.multiplier_2, out var results);
            if (didPurchase)
            {
                SummoningManager.AddOpenedScrollCount(currentUI.id);
                ShowCardsOpening(results, _tokenSource.Token);
            }
        }

        private void OnCountX3()
        {
            if(!_canTakeInput) return;
            CLog.Log($"[{nameof(SummoningUI)}] OnCountX3. Id: {currentUI.id}. Count: {currentUI.multiplier_3}");
            var didPurchase = SummoningManager.TryGetOutputFromScroll(currentUI.id, (int)currentUI.multiplier_3, 
                currentUI.multiplier_3, out var results);
            if (didPurchase)
            {
                SummoningManager.AddOpenedScrollCount(currentUI.id);
                ShowCardsOpening(results, _tokenSource.Token);
            }
        }

        private async void ShowCardsOpening(List<SummonOutput> outputResults, CancellationToken token)
        {
            _canTakeInput = _canvas.enabled = false;
            try { await cardsDisplay.Show(outputResults, token); }
            catch (System.Exception ex)            
            {
                CLog.LogError($"{ex.Message}\n{ex.StackTrace}");
            }
            if (token.IsCancellationRequested) { return; }
            OnNewItemPicked();
            _canTakeInput = _canvas.enabled = true;
        }

        private void OnNewItemPicked()
        {
            var ui = (SummonOptionUI)_scroller.currentObject;
            if (ui == null)
            {
                CLog.LogError($"Cannot cast (SummonOptionUI) to current object inside scroller");
                return;
            }
            SetData(ui); 
            SetFreeBadgesOnScroll();
        }

        private void AdCallbacks(SummonOptionUI ui)
        {
            switch (ui.id)
            {
                case ItemsIds.Scroll1:
                    ui.btnOptional.AddMainCallback(OnScrollFree);
                    ui.btn1.AddMainCallback(OnCountX1);
                    ui.btn2.AddMainCallback(OnCountX2);
                    ui.btn3.AddMainCallback(OnCountX3);
                    break;
                case ItemsIds.Scroll2:
                    ui.btnOptional.AddMainCallback(OnScrollAd);
                    ui.btn2.AddMainCallback(OnCountX2);
                    ui.btn3.AddMainCallback(OnCountX3);
                    break;
                case ItemsIds.Scroll3:
                    ui.btn2.AddMainCallback(OnCountX2);
                    ui.btn3.AddMainCallback(OnCountX3);
                    break;
                case ItemsIds.Scroll4:
                    ui.btn2.AddMainCallback(OnCountX1);
                    ui.btn3.AddMainCallback(OnCountX1);
                    break;
            }
        }

        private void InitData()
        {
            foreach (var option in _options)
                SetData(option);
        }

        private void OnEnable()
        {
            _tokenSource = new CancellationTokenSource();
            _canTakeInput = true;
        }
        
        private void OnDisable()
        {
            _tokenSource?.Cancel();
        }
        
        private void Return()
        {
            if (!_canTakeInput) return;
            ScreenDarkening.Animate(() =>
            {
                _tokenSource?.Cancel();
                _canTakeInput = false;
                _scroller.OnNewItemChosen -= OnNewItemPicked;
                gameObject.SetActive(false);
                ServiceLocator.Get<IUIManager>().OnClosed(UIConstants.UISummon);
            });
        }

        private void SetData(SummonOptionUI optionUI)
        {
            var db = ServiceLocator.Get<SummoningDataBase>();
            var gm = ServiceLocator.Get<GameMoney>();
            int cost;
            int fullCost;
            var id = optionUI.id;
            var inventory = DataHelpers.GetInventory();
            var config = db.GetConfig(id);
            int money = SummoningManager.GetMoneyAmount(config.currencyId);
            var save = inventory.GetScrollSave(id);
            var ownedAmount = save.ownedAmount;
            optionUI.SetPurchasesCount(save.purchasedCount);
            optionUI.invView1.text.text = $"x{ownedAmount}";
            // CLog.Log($"On option picked. setting up: {id}");
            switch (id)
            {
                case ItemsIds.Scroll1:
                    money = SummoningManager.GetMoneyAmount(config.currencyId);
                    if ((config.adAvailable || config.freeAvailable)
                        && save.timerData.CheckIfTimePassed(TimeSpan.FromHours(config.timePeriodHours)) )
                    {
                        optionUI.btn1.Off();
                        optionUI.btnOptional.On(); 
                    }
                    else
                    {
                        optionUI.btn1.On();
                        optionUI.btnOptional.Off(); 
                        SetupButton(optionUI.btn1, optionUI.multiplier_1, config.purchaseCost);
                    }
                    
                    optionUI.invView2.text.text = money.ToString();
                    SetupButton(optionUI.btn2, optionUI.multiplier_2, config.purchaseCost);
                    SetupButton(optionUI.btn3, optionUI.multiplier_3, config.purchaseCost);
                    break;
                
                case ItemsIds.Scroll2:
                    if ((config.adAvailable || config.freeAvailable)
                        && save.timerData.CheckIfTimePassed(TimeSpan.FromHours(config.timePeriodHours)) )
                    {
                        optionUI.btnOptional.SetInteractable(true);
                    }
                    else
                    {
                        optionUI.btnOptional.SetInteractable(false);
                    }
                    optionUI.invView2.text.text = money.ToString();
                    SetupButton(optionUI.btn2, optionUI.multiplier_2, config.purchaseCost);
                    SetupButton(optionUI.btn3, optionUI.multiplier_3, config.purchaseCost);
                    break;
                    
                case ItemsIds.Scroll3:
                    optionUI.invView2.text.text = money.ToString();
                    SetupButton(optionUI.btn2, optionUI.multiplier_2, config.purchaseCost);
                    
                    fullCost = (int)(config.purchaseCost * optionUI.multiplier_3);
                    if (money >= fullCost)
                        optionUI.btn3.SetPriceEnough(fullCost);
                    else
                        optionUI.btn3.SetPriceNotEnough(fullCost);
                    break;  
                
                case ItemsIds.Scroll4:
                    cost = config.purchaseCost;
                    optionUI.invView2.text.text = money.ToString();
                    optionUI.btn2.SetAsUseItem(1);
                    
                    if (money >= cost)
                        optionUI.btn3.SetPriceEnough(cost);
                    else
                        optionUI.btn3.SetPriceNotEnough(cost);
                    break;  
            }

            void SetupButton(PurchaseButton btn, float multiplier, int costOfOne)
            {
                if (save.ownedAmount >= multiplier)
                {
                    btn.SetAsUseItem((int)multiplier);
                }
                else
                {
                    fullCost = (int)(costOfOne * multiplier);
                    if (money >= fullCost)
                        btn.SetPriceEnough(fullCost);
                    else
                        btn.SetPriceNotEnough(fullCost);
                }
            }
        }
        
    }
    
}