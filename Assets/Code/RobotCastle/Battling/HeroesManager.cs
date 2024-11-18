using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.Saving;
using SleepDev;
using SleepDev.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    public static class HeroesManager
    {
        public static void UpdateWeaponModifiers(GameObject hero)
        {
            var components = hero.gameObject.GetComponent<HeroComponents>();
            components.stats.ClearDecorators();
            hero.GetComponent<IHeroWeaponsContainer>().AddAllModifiersToHero(components);
        }

        public static string Red(int val) => $"<color=#BB0000>{val}</color>";

        public static string Red(string msg) => $"<color=#BB0000>{msg}</color>";

        public static void AddRewardOrBonus(CoreItemData data)
        {
            switch (data.type)
            {
                case ItemsIds.TypeItem:
                    AddItem(data);
                    break;
                case ItemsIds.TypeBonus:
                    AddBonus(data);
                    break;
            }
        }

        public static void AddItem(CoreItemData data)
        {
            var merge = ServiceLocator.Get<MergeManager>();
            var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
            factory.SpawnHeroOrItem(new SpawnArgs(data), merge.GridView, merge.SectionsController, out var item);
        }

        public static void AddBonus(CoreItemData data)
        {
            switch (data.id)
            {
                case ItemsIds.IdBonusTroops:
                    CLog.Log($"[bonus={data.id}] Adding troops size by 1");
                    var troops = ServiceLocator.Get<ITroopSizeManager>();
                    troops.ExtendBy(data.level);
                    break;
                case ItemsIds.IdRestoreHealth:
                    CLog.Log($"[bonus={data.id}] Adding health");
                    var battleManager = ServiceLocator.Get<BattleManager>();
                    var health = battleManager.battle.playerHealthPoints;
                    health += data.level;
                    battleManager.battle.playerHealthPoints = health;
                    ServiceLocator.Get<CastleHealthView>().AddHealth(health);
                    break;
                case ItemsIds.IdBonusMoney:
                    CLog.Log($"[bonus={data.id}] Adding money +{data.level}");
                    var gm = ServiceLocator.Get<GameMoney>();
                    gm.AddMoney(data.level);
                    break;
                case ItemsIds.IdHeroLevelUp:
                    CLog.Log($"[bonus={data.id}] Random hero level up");
                    var merge = ServiceLocator.Get<MergeManager>();
                    var heroes = merge.SectionsController.GetAllItemsViews()
                        .FindAll(t => t.itemData.core.type == ItemsIds.TypeHeroes && (t.itemData.core.level + 1) < MergeConstants.HeroMaxMergeLvl);
                    if (heroes.Count == 0)
                    {
                        CLog.Log($"Heroes count is 0. Cannot level up");
                        return;
                    }

                    var chosenHero = heroes.Random();
                    chosenHero.itemData.core.level++;
                    var animation = ServiceLocator.Get<MergeAnimation>();
                    animation.PlayUpgradeOnly(chosenHero);
                    break;
                
                case ItemsIds.IdAdvancedSummon:
                    var factory = ServiceLocator.Get<IPlayerFactory>();
                    factory.AdvancedScrollsCount.AddValue(data.level);
                    factory.SummonHeroLevel = 1;
                    break;
            }
        }


        public static async Task WaitGameTime(float seconds, CancellationToken token)
        {
            while (seconds >= 0 && !token.IsCancellationRequested)
            {
                seconds -= Time.deltaTime;
                await Task.Yield();
            }
        }
        
        public static async Task WaitGameTimeUnscaled(float seconds, CancellationToken token)
        {
            while (seconds >= 0 && !token.IsCancellationRequested)
            {
                seconds -= Time.unscaledDeltaTime;
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
        public static float GetDef(float def) => (def) / (900 + def);

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
            if(!string.IsNullOrEmpty(spellInfo.mainSpellId))
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
                    if (hh.IsDead || hh.Components.state.isOutOfMap)
                        continue;
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
                    if (hh.IsDead || hh.Components.state.isOutOfMap)
                        continue;
                    var d2 = (cellWorldPos - hh.Components.transform.position).sqrMagnitude;
                    if (d2 <= rad2)
                        result.Add(hh);   
                }
            }
            return result;
        }

        public static List<IHeroController> GetHeroesInsideCellMask(
            CellsMask cellsMask, 
            Vector2Int center,
            IMap map,
            IList<IHeroController> allHeroes,
            float cellRadius = .5f)
        {
            var affected = cellsMask.GetCellsAround(center, map);
            var rad2 = cellRadius * cellRadius;
            var result = new List<IHeroController>(allHeroes.Count);
            foreach (var cell in affected)
            {
                var cellWorldPos = map.GetWorldFromCell(cell);
                foreach (var hh in allHeroes)
                {
                    if (hh.IsDead || hh.Components.state.isOutOfMap)
                        continue;
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
                    if (hh.IsDead || hh.Components.state.isOutOfMap)
                        continue;
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

        public static (bool, Vector2Int) GetClosestFreeCell(Vector2Int center, IMap map)
        {
            Vector2Int result = default;
            var maxRad = map.Size.x >= map.Size.y ? map.Size.x : map.Size.y;
            for (var rad = 1; rad < maxRad; rad++)
            {
                int x = 0;
                int y = rad;
                for (x = -rad; x <= rad; x++)
                {
                    var cell = center + new Vector2Int(x, y);
                    if (map.IsFullyFree(cell))
                    {
                        result = cell;
                        return (true, result);
                    }
                }

                y = -rad;
                for (x = -rad; x <= rad; x++)
                {
                    var cell = center + new Vector2Int(x, y);
                    if (map.IsFullyFree(cell))
                    {
                        result = cell;
                        return (true, result);
                    }
                }

                x = rad;
                for (y = rad - 1; y > -rad; y--)
                {
                    var cell = center + new Vector2Int(x, y);
                    if (map.IsFullyFree(cell))
                    {
                        result = cell;
                        return (true, result);
                    }
                }
                
                x = -rad;
                for (y = rad - 1; y > -rad; y--)
                {
                    var cell = center + new Vector2Int(x, y);
                    if (map.IsFullyFree(cell))
                    {
                        result = cell;
                        return (true, result);
                    }
                }
            }
            return (false, result);
        }
        
        
    }
}