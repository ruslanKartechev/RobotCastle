using System.Collections.Generic;
using Bomber;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using UnityEngine;

namespace RobotCastle.Battling
{
    public static class HeroesHelper
    {
        public static float ReduceDamageByDef(float damage, float def)
        {
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

        public static List<IHeroController> GetHeroesEnemies(HeroView view)
        {
            var cc = view.gameObject.GetComponent<IHeroController>();
            return cc.Battle.GetTeam(cc.TeamNum).enemyUnits;
        }

        public static List<IHeroController> GetHeroesAllies(HeroView view)
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
                    var d2 = (cellWorldPos - hh.View.transform.position).sqrMagnitude;
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
                    var d2 = (cellWorldPos - hh.View.transform.position).sqrMagnitude;
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
                    var d2 = (heroPos - hh.View.transform.position).sqrMagnitude;
                    if (d2 <= rad2)
                        return true;
                }
            }
            return false;
        }
    }
}