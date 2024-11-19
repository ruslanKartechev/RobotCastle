using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Shop
{
    public enum EPurchaseResult {Success, NotEnoughMoney, Error}

    public class ShopManager : IShopManager
    {
        public bool GrandItem(CoreItemData item)
        {
            switch (item.type)
            {
                case ItemsIds.TypeHeroes:
                    GrantXp(item.id, item.level);
                    break;
                case ItemsIds.TypeBonus:
                    break;
                case ItemsIds.TypeItem:
                    GrantById(item.id, item.level);
                    break;
                default:
                    CLog.LogError($"Unknown item type: {item.type}");
                    return false;
            }
            return true;
        }

        private void GrantXp(string heroId, int xp)
        {
            var saves = DataHelpers.GetHeroesSave();
            var save = saves.heroSaves.Find(t => t.id == heroId);
            save.xp += xp;
        }

        private void GrantById(string id, int count)
        {
            switch (id)
            {
                case ItemsIds.IdMoney:
                {
                    var gm = ServiceLocator.Get<GameMoney>();
                    gm.globalMoney.AddValue(count);
                }
                    break;
                case ItemsIds.IdHardMoney:
                {
                    var gm = ServiceLocator.Get<GameMoney>();
                    gm.globalHardMoney.AddValue(count);
                }
                    break;
                default:
                    var inv = DataHelpers.GetInventory();
                    inv.AddItem(id, count);
                    break;
                

            }
        }

        
        
    }
}