using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using SleepDev.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    public static class HeroesManager
    {

        public static async Task WaitGameTime(float seconds, CancellationToken token)
        {
            while (seconds >= 0 && !token.IsCancellationRequested)
            {
                seconds -= Time.deltaTime;
                await Task.Yield();
            }
        }
        
        public const int MaxHeroLevel = 20;
        
        public static float ReduceDamageByDef(float damage, float def)
        {
            if (def <= 0)
                return damage;
            damage -= GetDef(def) * damage;
            return damage;
        }
        
        /// <summary>
        /// Always less than 1
        /// </summary>
        public static float GetDef(float def)
        {
            return (def) / (900 + def);
        }
        
        public static ESpellTier GetSpellTier(int mergeLevel)
        {
            for (var i = HeroesConstants.SpellTiersByMergeLevel.Count - 1; i >= 0; i--)
            {
                if (mergeLevel >= HeroesConstants.SpellTiersByMergeLevel[i])
                    return (ESpellTier)i;
            }
            return (ESpellTier)0;
        }
        
        public static int GetHeroLevel(string id)
        {
            var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(id);
            return heroSave.level;
        }

        public static List<ModifierProvider> GetModifiers(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<ModifierProvider>();
            var db = ServiceLocator.Get<ModifiersDataBase>();
            var list = new List<ModifierProvider>(3);
            foreach (var id in ids)
                list.Add(db.GetModifier(id));
            return list;
        }

        public static List<IHeroController> GetHeroesEnemies(HeroComponents view)
        {
            var cc = view.gameObject.GetComponent<IHeroController>();
            return cc.Battle.GetTeam(cc.TeamNum).enemyUnits;
        }

        public static List<IHeroController> GetHeroesAllies(HeroComponents view)
        {
            var cc = view.gameObject.GetComponent<IHeroController>();
            return cc.Battle.GetTeam(cc.TeamNum).ourUnits;
        }
        
        public static List<ModifierProvider> GetModifiersForHero(string id)
        {
            var spellInfo = ServiceLocator.Get<HeroesDatabase>().GetHeroSpellInfo(id);
            var db = ServiceLocator.Get<ModifiersDataBase>();
            var list = new List<ModifierProvider>(3);
            if(spellInfo.mainSpellId.Length > 0)
                list.Add(db.GetSpell(spellInfo.mainSpellId));
            return list;
        }

        
        public static (List<IHeroController>, List<Vector2Int> ) GetCellsHeroesInsideCellMask(
            CellsMask cellsMask, 
            Vector3 worldCenter,
            IMap map,
            IList<IHeroController> allHeroes,
            float cellRadius = .5f)
        {
            var center = map.GetCellPositionFromWorld(worldCenter);
            var affected = cellsMask.GetCellsAround(center, map);
            var rad2 = cellRadius * cellRadius;
            var result = new List<IHeroController>(allHeroes.Count);
            foreach (var cell in affected)
            {
                var cellWorldPos = map.GetWorldFromCell(cell);
                foreach (var hh in allHeroes)
                {
                    var d2 = (cellWorldPos - hh.Components.transform.position).sqrMagnitude;
                    if (d2 <= rad2)
                        result.Add(hh);   
                }
            }
            return (result, affected);
        }
        
        public static List<IHeroController> GetHeroesInsideCellMask(
            CellsMask cellsMask, 
            Vector3 worldCenter,
            IMap map,
            IList<IHeroController> allHeroes,
            float cellRadius = .5f)
        {
            var center = map.GetCellPositionFromWorld(worldCenter);
            var affected = cellsMask.GetCellsAround(center, map);
            var rad2 = cellRadius * cellRadius;
            var result = new List<IHeroController>(allHeroes.Count);
            foreach (var cell in affected)
            {
                var cellWorldPos = map.GetWorldFromCell(cell);
                foreach (var hh in allHeroes)
                {
                    var d2 = (cellWorldPos - hh.Components.transform.position).sqrMagnitude;
                    if (d2 <= rad2)
                        result.Add(hh);   
                }
            }
            return result;
        }

        
        public static bool CheckIfAtLeastOneHeroInMask(
            CellsMask cellsMask, 
            Vector2Int center,
            IMap map,
            IList<IHeroController> allHeroes,
            float cellRadius = 1f)
        {
            var affected = cellsMask.GetCellsAround(center, map);
            var rad2 = cellRadius * cellRadius;
            foreach (var cell in affected)
            {
                var heroPos = map.GetWorldFromCell(cell);
                foreach (var hh in allHeroes)
                {
                    var d2 = (heroPos - hh.Components.transform.position).sqrMagnitude;
                    if (d2 <= rad2)
                        return true;
                }
            }
            return false;
        }
        
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
        
        /// <summary>
        /// </summary>
        /// <param name="heroSave"></param>
        /// <returns> 0 - success, 1 - not enough xp, 2 - not enough money</returns>
        public static int UpgradeHero(HeroSave heroSave)
        {
            if (heroSave.level >= MaxHeroLevel)
                return 3;
            if(heroSave.xpForNext > heroSave.xp)
                return 1;
            var gm = ServiceLocator.Get<GameMoney>();
            var money = gm.globalMoney.Val;
            var db = ServiceLocator.Get<XpDatabase>();
            var cost = db.heroesUpgradeCosts[heroSave.level];
            if (cost > money)
                return 2;
            money -= cost;
            gm.globalMoney.UpdateWithContext(money, (int)EMoneyChangeContext.AfterPurchase);
            heroSave.xp -= heroSave.xpForNext;
            heroSave.level++;
            if (heroSave.level < MaxHeroLevel)
                heroSave.xpForNext = db.heroXpLevels[heroSave.level];
            return 0;
        }

        public static void UpgradeNoCharge(HeroSave heroSave)
        {
            if (heroSave.level >= MaxHeroLevel)
                return;
            var db = ServiceLocator.Get<XpDatabase>();
            heroSave.xp -= heroSave.xpForNext;
            if(heroSave.xp < 0)
                heroSave.xp = 0;
            heroSave.level++;
            if (heroSave.level < MaxHeroLevel)
                heroSave.xpForNext = db.heroXpLevels[heroSave.level];
        }
        
    }
}