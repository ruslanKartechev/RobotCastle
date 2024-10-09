using System;
using RobotCastle.Battling;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.UI
{
    public partial class HeroGrowthPanel
    {
        private class PurchaseUpgradeOperation : IOperation
        {
            public HeroSave heroSave;
            public Action<Vector2Int, Vector2Int, float, float> animateXpCallback;
            public Action upgradeStatsCallback;
            public Action upgradeLevelCallback;

            public PurchaseUpgradeOperation(HeroSave heroSave, Action<Vector2Int, Vector2Int, float, float> animateXpCallback, Action upgradeStatsCallback, Action upgradeLevelCallback)
            {
                this.heroSave = heroSave;
                this.animateXpCallback = animateXpCallback;
                this.upgradeStatsCallback = upgradeStatsCallback;
                this.upgradeLevelCallback = upgradeLevelCallback;
            }

            public int Apply()
            {
                var prevXp = new Vector2Int(heroSave.xp, heroSave.xpForNext);
                var percent1 = (float)prevXp.x / prevXp.y;

                var result = HeroesManager.UpgradeHero(heroSave);
                if (result == 0)
                {
                    var newXp = new Vector2Int(heroSave.xp, heroSave.xpForNext);
                    var percent2 = (float)newXp.x / newXp.y;
                    upgradeStatsCallback.Invoke();
                    upgradeLevelCallback.Invoke();
                    animateXpCallback.Invoke(prevXp, newXp, percent1, percent2);
                }
                return result;
            }
        }
    }
}