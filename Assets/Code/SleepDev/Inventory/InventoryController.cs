﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SleepDev.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        public event Action<Item> OnNewPicked; 
        public event Action OnNothingPicked; 

        public List<Item> AllItems => _items;
        
        public Item PickedItem => _pickedItem;
        
        [SerializeField] private List<Item> _items;
        [SerializeField] private GraphicRaycaster _raycaster;
        private Item _pickedItem;

        public void Reset()
        {
            if(_pickedItem != null)
                _pickedItem.Unpick();
            _pickedItem = null;
        }
        

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Click();
            }
        }
        
        private void Click()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var list = new List<RaycastResult>(10);
            _raycaster.Raycast(eventData, list);
            foreach (var res in list)
            {
                if (res.gameObject.TryGetComponent<Item>(out var item))
                {
                    if (item != _pickedItem)
                    {
                        if(_pickedItem != null)
                            _pickedItem.Unpick();
                        _pickedItem = item;
                        _pickedItem.Pick();
                        OnNewPicked?.Invoke(_pickedItem);
                    }
                    else
                    {
                        _pickedItem.Unpick();
                        _pickedItem = null;
                        OnNothingPicked?.Invoke();
                    }
                    return;
                }
            }
        }
        

    }
}