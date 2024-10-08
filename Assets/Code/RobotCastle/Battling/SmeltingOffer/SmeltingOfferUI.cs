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
        private List<CoreItemData> _options;
        private Action<CoreItemData> _callback;

        public void ShowOffer(List<CoreItemData> options, Action<CoreItemData> callback)
        {
            if (options.Count != _itemsUI.Count)
            {
                CLog.LogError($"Options count and ui elements count don't match!");
                return;
            }
            _callback = callback;
            _options = options;
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                _itemsUI[i].icon.sprite = viewDb.GetItemSpriteByTypeAndLevel(option);
                _itemsUI[i].NumberId = i;
                _itemsUI[i].AnimateShow();
            }
            SetClear();
            _confirmButton.SetInteractable(false);
            _confirmButton.AddMainCallback(OnConfirmBtn);
            _txtDescription.text = "";
            _inventory.OnNewPicked -= OnItemPicked;
            _inventory.OnNothingPicked -= OnNothingPicked;
            _inventory.OnNewPicked += OnItemPicked;
            _inventory.OnNothingPicked += OnNothingPicked;
        }

        private void OnNothingPicked()
        {
            CLog.Log($"[OnNothingPicked]");
            SetClear();
            _confirmButton.SetInteractable(false);
        }

        private void SetClear()
        {
            _pickedItemIcon.enabled = false;
            _txtDescription.text = "";
            _txtItemLevel.text = "";
            _txtItemName.text = "";
        }

        private void OnItemPicked(SleepDev.Inventory.Item item)
        {
            CLog.Log($"[OnItemPicked] {item.NumberId}");
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
            _callback.Invoke(_options[_inventory.PickedItem.NumberId]);
            gameObject.SetActive(false);
            ServiceLocator.Get<IUIManager>().OnClosed(UIConstants.UISmeltingOffer);
        }
        
    }
}