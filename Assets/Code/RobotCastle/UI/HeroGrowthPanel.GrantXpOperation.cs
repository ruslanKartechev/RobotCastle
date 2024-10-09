using System;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev.Inventory;
using UnityEngine;

namespace RobotCastle.UI
{
    public partial class HeroGrowthPanel
    {
        private class GrantXpOperation : IOperation
        {
            public HeroSave heroSave;
            public Item itemUI;
            public float needXpAmount;
            public int needItemsCount;
            public int giveXpAmount;
            public int consumeItemsCount;
            public Action<Vector2Int, Vector2Int, float, float> animateXpCallback;
            
            public GrantXpOperation(HeroSave heroSave, Item itemUI, 
                Action<Vector2Int, Vector2Int, float, float> animateXpCallback)
            {
                this.itemUI = itemUI;
                this.heroSave = heroSave;
                this.animateXpCallback = animateXpCallback;
                var xpdb = ServiceLocator.Get<XpDatabase>();
                var xpPerItem = xpdb.xpGrantedByItem[itemUI.Id];
                needXpAmount = (float)(this.heroSave.xpForNext - this.heroSave.xp);
                var totalItemsCount = itemUI.GetCount();
                var necessaryCount = Mathf.CeilToInt(needXpAmount / xpPerItem);
                if (totalItemsCount >= necessaryCount)
                {
                    giveXpAmount = Mathf.CeilToInt(needXpAmount);
                    consumeItemsCount = necessaryCount;
                }
                else
                {
                    consumeItemsCount = totalItemsCount;
                    giveXpAmount = Mathf.CeilToInt(totalItemsCount * xpPerItem);
                }
            }

            public int Apply()
            {
                var prevXp = new Vector2Int(heroSave.xp, heroSave.xpForNext);
                var percent1 = (float)prevXp.x / prevXp.y;
                
                heroSave.xp += giveXpAmount;
                
                var newXp = new Vector2Int(heroSave.xp, heroSave.xpForNext);
                var percent2 = (float)newXp.x / newXp.y;
                
                var count = itemUI.GetCount();
                itemUI.SetCount(count - consumeItemsCount);
                animateXpCallback.Invoke(prevXp, newXp, percent1, percent2);
                return 0;
            }
        }
    }
}