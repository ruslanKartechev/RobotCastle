using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Shop
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private TextMeshProUGUI _amountGivenText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private MyButton _btn;
        
        public ShopItemData itemData { get; set; }
        

        private void OnEnable()
        {
            _btn.AddMainCallback(TryPurchase);
        }

        private void TryPurchase()
        {
            CLog.Log($"Trying to purchase");
        }

        public void SetupItem(ShopItemData itemData)
        {
            this.itemData = itemData;
            
            switch (itemData.itemData.type)
            {
                case ItemsIds.TypeHeroes:
                    var heroesDb = ServiceLocator.Get<HeroesDatabase>();
                    var viewinfo = heroesDb.GetHeroViewInfo(itemData.itemData.id);
                    _nameText.text = viewinfo.name;
                    _itemIcon.sprite = ViewDataBase.GetHeroSprite(viewinfo.iconId);
                    break;
                default:
                    var descriptionsDataBase = ServiceLocator.Get<DescriptionsDataBase>();
                    _nameText.text = descriptionsDataBase.GetDescription(itemData.itemData.id).parts[0];
                    break;
            }
            _amountGivenText.text = $"+{itemData.itemData.level.ToString()}";

            switch (itemData.currency)
            {
                case EShopCurrency.RealWorldMoney:
                    CLog.Log($"In app Case!");
                    break;
                default:
                    _costText.text = itemData.cost.ToString();
                    break;
            }

        }

    }
}