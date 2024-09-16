using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Merging
{
    public class ChooseItemsOffer
    {
        private IItemsChoiceListener _listener;
        public int MaxItemsCount { get; set; } = 3;

        public ChooseItemsOffer(int maxCount, IItemsChoiceListener listener)
        {
            MaxItemsCount = maxCount;
            _listener = listener;
        }
            
        public void OfferChooseItems(List<CoreItemData> items)
        {
            CLog.Log($"[ChooseItemsOffer] All items (total: {items.Count}) won't fit");
            var ui = ServiceLocator.Get<IUIManager>().Show<ChooseUnitItemsUI>(UIConstants.UIPickUnitItems, () => {});
            ui.PickMaximum(items, MaxItemsCount, _listener);
        }
    }
}