using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using SleepDev.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Battling.SmeltingOffer
{
    public class SmeltingOfferUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private TextMeshProUGUI _txtDescription;
        [SerializeField] private TextMeshProUGUI _txtItemName;
        [SerializeField] private TextMeshProUGUI _txtItemLevel;
        [SerializeField] private Image _pickedItemIcon;
        [SerializeField] private List<SmeltingItemUI> _itemsUI;
        [SerializeField] private InventoryController _inventory;
        [SerializeField] private MyButton _confirmButton;
        [SerializeField] private TextMeshProUGUI _rerollsText;
        [SerializeField] private MyButton _rerollsBtn;
        [SerializeField] private BlackoutFadeScreen _fadeScreen;

        private List<CoreItemData> _options;
        private SmeltingOfferManager _manager;

        public void ShowOffer(List<CoreItemData> options, SmeltingOfferManager manager)
        {
            if (options.Count != _itemsUI.Count)
            {
                CLog.LogError($"Options count and ui elements count don't match!");
                return;
            }
            gameObject.SetActive(true);
            _fadeScreen.FadeInWithId(UIConstants.UISmeltingOffer);
            _manager = manager;
            _options = options;

            SetRerolls();
            ShowOptions(options);
            SetClear();
            _confirmButton.SetInteractable(false);
            _confirmButton.AddMainCallback(OnConfirmBtn);
            _txtDescription.text = "";
            _inventory.OnNewPicked -= OnItemPicked;
            _inventory.OnNothingPicked -= OnNothingPicked;
            _inventory.OnNewPicked += OnItemPicked;
            _inventory.OnNothingPicked += OnNothingPicked;
        }

        private void ShowOptions(List<CoreItemData> options)
        {
            _options = options;
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                _itemsUI[i].icon.sprite = viewDb.GetItemSpriteByTypeAndLevel(option);
                _itemsUI[i].NumberId = i;
                _itemsUI[i].AnimateShow();
            }
        }

        private void RerollBtn()
        {
            OnNothingPicked();
            _inventory.SetNoItem();
            var options = _manager.RerollItems();
            if (options == null)
            {
                SetRerolls();
                return;
            }
            ShowOptions(options);
            SetRerolls();
        }

        private void SetRerolls()
        {
            var rerollsCount = _manager.Rerolls;
            _rerollsText.text = $"Rerolls: {rerollsCount}";
            if (rerollsCount > 0)
            {
                _rerollsBtn.gameObject.SetActive(true);
                _rerollsBtn.AddMainCallback(RerollBtn);
            }
            else
                _rerollsBtn.gameObject.SetActive(false);
        }
        
        private void OnNothingPicked()
        {
            SetClear();
            _confirmButton.SetInteractable(false);
        }

        private void SetClear()
        {
            _pickedItemIcon.enabled = false;
            _txtDescription.text = "";
            _txtItemLevel.text = "";
            _txtItemName.text = "Pick One Item";
        }

        private void OnItemPicked(SleepDev.Inventory.Item item)
        {
            // CLog.Log($"[OnItemPicked] {item.NumberId}");
            var option = _options[item.NumberId];
            var descrDb = ServiceLocator.Get<DescriptionsDataBase>();
            var dd = descrDb.GetDescriptionByTypeAndLevel(option);
            _txtItemName.text = dd.parts[0];
            _txtItemLevel.text = dd.parts[1];
            _txtDescription.text = dd.parts[2];
            _pickedItemIcon.sprite = _itemsUI[item.NumberId].icon.sprite;
            _pickedItemIcon.enabled = true;
            _confirmButton.SetInteractable(true);
        }

        private void OnConfirmBtn()
        {
            CLog.Log($"[OnConfirmBtn]");
            if (_inventory.PickedItem == null)
            {
                CLog.LogError($"NO item picked");
                return;
            }
            _manager.OnChoiceConfirmed(_options[_inventory.PickedItem.NumberId]);
            _fadeScreen.FadeOut();
        }
        
    }
}