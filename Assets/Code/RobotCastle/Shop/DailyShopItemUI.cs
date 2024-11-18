using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Shop
{
    public class DailyShopItemUI : MonoBehaviour
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
        
        public IShopManager shopManager { get; set; }

        public void SetData(CoreItemData core)
        {
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
        [SerializeField] private MyButton _btn;
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
                    shopManager.GrandItem(itemData);
                    itemSave.isAvailable = false;
                }    
            });
        }

    }
}