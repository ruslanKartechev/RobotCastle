using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using SleepDev.FlyingUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Shop
{
    public class DailyShopItemUI : ShopItemUI
    {
        public CoreItemData itemData { get; private set; }
        
        public ShopItemSave itemSave
        {
            get => _save;
            set
            {
                _save = value;
                SetAvailable(_save.isAvailable);
            } }
        
        public void SetData(CoreItemData core)
        {
            itemData = core;
            var db = ServiceLocator.Get<ViewDataBase>();
            string title = "";
            switch (core.type)
            {
                case ItemsIds.TypeHeroes:
                    var heroesDb = ServiceLocator.Get<HeroesDatabase>();
                    var info = heroesDb.GetHeroInfo(core.id).viewInfo;
                    title = info.name;
                    _itemIcon.sprite = ViewDataBase.GetSprite(info.iconId);
                    
                    break;
                case ItemsIds.TypeItem or ItemsIds.TypeBonus:
                    _itemIcon.sprite = db.GetGeneralItemSprite(core.id);
                    var descr = ServiceLocator.Get<DescriptionsDataBase>();
                    title = descr.GetDescription(core.id).parts[0];
                    break;
                default:
                    CLog.LogError($"Unknown type! {core.type}");
                    break;
            }
            _amountGivenText.text = $"+{core.level}";
            _nameText.text = title;
            _purchaseMaker = gameObject.GetComponent<IShopPurchaseMaker>();
        }

        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _amountGivenText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private List<GameObject> _blockingObjects;
        private IShopPurchaseMaker _purchaseMaker;
        private ShopItemSave _save;
        
        private void OnEnable()
        {
            _btn.AddMainCallback(TryPurchase);
        }

        private void SetAvailable(bool yes)
        {
            _btn.SetInteractable(yes);
            foreach (var go in _blockingObjects)
                go.SetActive(!yes);
        }

        private void TryPurchase()
        {
            CLog.Log($"[{gameObject.name}] Trying to purchase");
            
            _purchaseMaker.TryPurchase((result) =>
            {
                CLog.Log($"Purchase result: {result.ToString()}");
                if (result == EPurchaseResult.Success)
                {
                    itemSave.isAvailable = false;
                    ServiceLocator.Get<IShopManager>().GrandItem(itemData);
                    SetAvailable(false);
                    var flyPos = new Vector3(Screen.width / 2f, Screen.height, 0);
                    var flyingUI = ServiceLocator.Get<FlyingUIScreen>();
                    flyingUI.FlyFromTo(_itemIcon.sprite, transform.position, flyPos, 3);
                    
                }    
            });
        }
    }

}