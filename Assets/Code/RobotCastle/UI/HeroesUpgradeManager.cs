using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;

namespace RobotCastle.UI
{
    public class HeroesUpgradeManager
    {
        public const int MaxHeroLevel = 20;
        
        public static void CheckAllHeroesXp(List<HeroSave> allHeroes)
        {
            var db = ServiceLocator.Get<XpDatabase>();
            foreach (var heroSave in allHeroes)
            {
                if (!heroSave.isUnlocked)
                {
                    heroSave.xpForNext = 5;
                    heroSave.xp = 0;
                }
                else
                {
                    heroSave.xpForNext = db.heroXpLevels[heroSave.level];
                }
            }
        }
        
        /// <summary>
        /// </summary>
        /// <param name="heroSave"></param>
        /// <returns>0 - can, 1 - not enough xp, 2 - not enough money, 3 - maxLvl </returns>
        public static int CanUpgrade(HeroSave heroSave, int money, out int upgCost)
        {
            upgCost = 0;
            if (heroSave.level >= MaxHeroLevel)
                return 3;
            var db = ServiceLocator.Get<XpDatabase>();
            var cost = db.heroesUpgradeCosts[heroSave.level];
            upgCost = cost;
            heroSave.xpForNext = db.heroXpLevels[heroSave.level];
            if (heroSave.xp < heroSave.xpForNext)
                return 1;
            if (cost > money)
                return 2;
            return 0;
        }
        
        public static void UpgradeHero(HeroSave heroSave)
        {
            if (heroSave.level >= MaxHeroLevel)
                return;
            var gm = ServiceLocator.Get<GameMoney>();
            var money = gm.globalMoney;
            var db = ServiceLocator.Get<XpDatabase>();
            var cost = db.heroesUpgradeCosts[heroSave.level];
            money -= cost;
            gm.globalMoney = money;
            heroSave.xp -= heroSave.xpForNext;
            heroSave.level++;
            if (heroSave.level < MaxHeroLevel)
                heroSave.xpForNext = db.heroXpLevels[heroSave.level];
        }

        public static void UpgradeNoCharge(HeroSave heroSave)
        {
            if (heroSave.level >= MaxHeroLevel)
                return;
            var db = ServiceLocator.Get<XpDatabase>();
            heroSave.xp -= heroSave.xpForNext;
            heroSave.level++;
            if (heroSave.level < MaxHeroLevel)
                heroSave.xpForNext = db.heroXpLevels[heroSave.level];
        }
    }
}