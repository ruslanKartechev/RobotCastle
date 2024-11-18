using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class OfferedItemDescriptionUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _txtDescription;
        [SerializeField] private TextMeshProUGUI _txtItemName;
        [SerializeField] private TextMeshProUGUI _txtItemLevel;
        [SerializeField] private ItemDescriptionShortUI _itemDescription;

        public void Clear()
        {
            _txtDescription.text = "";
            _txtItemName.text = "";
            _txtItemLevel.text = "";
        }

        public void SetName(string msg) => _txtItemName.text = msg;

        public void StatsDescription(bool show) => _itemDescription.gameObject.SetActive(show);

        public void SetIcon(CoreItemData option)
        {
            var db = ServiceLocator.Get<ViewDataBase>();
            _icon.sprite = db.GetItemSpriteByTypeAndLevel(option);
        }
        
        public void ShowDescription(CoreItemData option)
        {
            switch (option.type)
            {
                case ItemsIds.TypeBonus:
                    ShowBonus(option);
                    break;
                case ItemsIds.TypeItem:
                    ShowItem(option);
                    break;
                default:
                    ShowDefault(option);
                    break;
                
            }
        }        
        
        public void ShowItem(CoreItemData option)
        {
            var descrDb = ServiceLocator.Get<DescriptionsDataBase>();
            var dd = descrDb.GetDescriptionByTypeAndLevel(option);
            _txtItemName.text = dd.parts[0];
            _txtItemLevel.text = dd.parts[1];
            _txtDescription.text = dd.parts[2];
            var modifiers = ServiceLocator.Get<ModifiersDataBase>().GetModifiersIdsForWeapon(option.id, option.level);
            if(_itemDescription.ShowStats(modifiers))
                StatsDescription(true);
            else
                StatsDescription(false);
        }

        private void ShowBonus(CoreItemData option)
        {
            StatsDescription(false);
            var descrDb = ServiceLocator.Get<DescriptionsDataBase>();
            var dd = descrDb.GetDescription(option.id);
            var tierStr = option.level.ToString();
            switch (option.id)
            {
                case ItemsIds.IdBonusMoney:
                    _txtItemName.text = dd.parts[0];
                    _txtDescription.text = dd.parts[1].Replace("<amount>", tierStr);
                    _txtItemLevel.text = "+" + tierStr;
                    break;
                default:
                    _txtItemName.text = dd.parts[0];
                    _txtDescription.text = dd.parts[1];
                    _txtItemLevel.text = "+" + tierStr;
                    break;
            }
        }
        
        private void ShowDefault(CoreItemData option)
        {
            StatsDescription(false);
            var descrDb = ServiceLocator.Get<DescriptionsDataBase>();
            var dd = descrDb.GetDescriptionByTypeAndLevel(option);
            _txtItemName.text = dd.parts[0];
            _txtItemLevel.text = dd.parts[1];
            _txtDescription.text = dd.parts[2];
        }


    }
}