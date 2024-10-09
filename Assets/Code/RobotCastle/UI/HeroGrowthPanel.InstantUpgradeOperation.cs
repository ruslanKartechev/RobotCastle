using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev.Inventory;

namespace RobotCastle.UI
{
    public partial class HeroGrowthPanel
    {
        private class InstantUpgradeOperation : IOperation
        {
            public Item item;
            public HeroSave heroSave;

            public InstantUpgradeOperation(HeroSave heroSave, Item item)
            {
                this.heroSave = heroSave;
                this.item = item;
            }
            
            public int Apply()
            {
                HeroesManager.UpgradeNoCharge(heroSave);
                var count = item.GetCount();
                item.SetCount(count-1);
                return 0;
            }
        }
    }
}