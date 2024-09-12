using System;
using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ChooseUnitItemsUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private List<ChoosableUnitItemUI> _elements;
        [SerializeField] private TextMeshProUGUI _textCount;
        [SerializeField] private Button _confirmButton;
        private List<ChoosableUnitItemUI> _activeUI;
        private List<CoreItemData> _items;
        private IItemsChoiceListener _listener;
        private int _max;
        private int _count;
        private bool _takeInput;

        public void PickMaximum(List<CoreItemData> items, int max, IItemsChoiceListener listener)
        {
            _listener = listener;
            _count = 0;
            _max = max;
            _items = items;
            foreach (var el in _elements)
                el.gameObject.SetActive(false);
            if (_elements.Count < items.Count)
            {
                throw new Exception("NOT ENOUGH ui elements _elements.Count < items.Count!");
            }
            _activeUI = new List<ChoosableUnitItemUI>(items.Count);
            var db = ServiceLocator.Get<ViewDataBase>();
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var ui = _elements[i];
                ui.Index = i;
                ui.SetIcon(db.GetUnitItemSpriteAtLevel(item.id, item.level));
                ui.Reset();
                ui.OnStateChange += OnStateChange;
                _activeUI.Add(ui);
                ui.AllowChoose = true;
                ui.gameObject.SetActive(true);
            }
            _textCount.text = $"{_count}/{_max}";
            _takeInput = true;
            gameObject.SetActive(true);
        }

        private void AllowPicking(bool allow)
        {
            foreach (var active in _activeUI)
                active.AllowChoose = allow;
        }
        
        private void OnStateChange()
        {
            var count = 0;
            foreach (var ui in _activeUI)
            {
                if (ui.IsChosen)
                    count++;
            }

            _count = count;
            _textCount.text = $"{_count}/{_max}";
            if (_count == _max)
            {
                AllowPicking(false);
                _confirmButton.interactable = true;
            }
            else if (_count < _max)
            {
                AllowPicking(true);
                _confirmButton.interactable = false;
            }
            else if (_count > _max)
            {
                AllowPicking(false);
                _confirmButton.interactable = false;
            }
        }

        private void OnConfirmBtn()
        {
            if (!_takeInput) return;
            if (_count == _max)
            {
                var chosenItems = new List<CoreItemData>(_max);
                var leftItems = new List<CoreItemData>(_max);
                
                foreach (var active in _activeUI)
                {
                    if(active.IsChosen)
                        chosenItems.Add(_items[active.Index]);
                    else
                        leftItems.Add(_items[active.Index]);
                }
                _listener.ConfirmChosenItems(chosenItems, leftItems);
                _takeInput = false;
                _confirmButton.interactable = false;
                gameObject.SetActive(false);
            }
            else
            {
                CLog.Log($"Count != Max!");
            }
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(OnConfirmBtn);
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(OnConfirmBtn);
        }
    }
}