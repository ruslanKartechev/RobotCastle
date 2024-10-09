using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class PlayerMergeItemPurchaser : MonoBehaviour, IPlayerMergeItemPurchaser
    {
        [SerializeField] private int _cost = 3;
        [SerializeField] private WeightedList<CoreItemData> _possibleItems;

        public void TryPurchaseItem(bool promptUser = true)
        {
            var gameMoney = ServiceLocator.Get<GameMoney>();
            var money = gameMoney.levelMoney;
            if (money < _cost)
            {
                CLog.Log($"[{nameof(PlayerMergeItemPurchaser)}] Not enough money");
                if (promptUser)
                {
                    var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {});
                    ui.ShowNotEnoughMoney();
                }
                return;
            }
            var item = _possibleItems.Random(true);
            var merge = ServiceLocator.Get<MergeManager>();
            var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
            var args = new SpawnMergeItemArgs(item);
            factory.SpawnHeroOrItem(args, merge.GridView, merge.SectionsController, out var spawnedItem);
            if (spawnedItem != null)
            {
                money -= _cost;
                gameMoney.levelMoney = money;
                merge.HighlightMergeOptions();
            }
        }
        
    }
}