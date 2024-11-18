using System;
using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Data
{
    [System.Serializable]
    public class ViewDataBase
    {
        public Dictionary<string, ItemInfo> ItemInfo = new (5);
        public Dictionary<string, string> StatsIcons = new (5);
        public Dictionary<string, string> SpellsIcons = new (5);
        public Dictionary<string, string> GeneralIcons = new (5);
        public List<string> LocationIcons = new(5);
        public List<string> LocationIconsCorruption = new(5);

        public static Sprite GetSprite(string spriteId) 
            => Resources.Load<Sprite>($"sprites/{spriteId}");

        public static string GetStatId(EStatType statType)
        {
            switch (statType)
            {
                case EStatType.Attack: return "attack";
                case EStatType.AttackSpeed: return "attack_speed";
                case EStatType.Health: return "health";
                case EStatType.MoveSpeed: return "move_speed";
                case EStatType.Range: return "range";
                case EStatType.SpellPower: return "spell_power";
            }
            throw new Exception($"{statType.ToString()} is not supported!");
        }

        public Sprite GetGeneralItemSprite(string spriteId)
        {
            return Resources.Load<Sprite>(GeneralIcons[spriteId]);
        }
        
        public Sprite GetStatIcon(EStatType statType)
        {
            return GetIconForStat(GetStatId(statType));
        }
        
        public Sprite GetSpellIcon(string id)
        {
            return Resources.Load<Sprite>(SpellsIcons[id]);
        }
        
        public Sprite GetIconForStat(string statId)
        {
            return Resources.Load<Sprite>(StatsIcons[statId]);
        }
        
        public GameObject GetMergePrefabAtLevel(string id, int levelIndex)
        {
            if (ItemInfo.ContainsKey(id))
            {
                var path = $"{ItemInfo[id].Prefab}_lvl_{levelIndex + 1}";
                return Resources.Load<GameObject>(path);
            }
            CLog.LogError($"[ViewDb] does not contain id {id}");
            return null;
        }
        
        public GameObject GetMergePrefab(string id)
        {
            if (ItemInfo.ContainsKey(id))
            {
                return Resources.Load<GameObject>(ItemInfo[id].Prefab);
            }
            CLog.LogError($"[ViewDb] does not contain id {id}");
            return null;
        }

        public Sprite GetWeaponSprite(string id, int levelIndex)
        {
            var path = $"{ItemInfo[id].Icon}_{levelIndex + 1}";
            return Resources.Load<Sprite>(path);
        }

        public Sprite GetItemSpriteByTypeAndLevel(CoreItemData itemData)
        {
            if(itemData.id.Contains(ItemsIds.IdAdvancedSummon))
                return Resources.Load<Sprite>(GeneralIcons[itemData.id]);
            switch (itemData.type)
            {
                case ItemsIds.TypeItem:
                    return GetWeaponSprite(itemData.id, itemData.level);
                case ItemsIds.TypeBonus:
                    return Resources.Load<Sprite>(GeneralIcons[$"{itemData.id}_{itemData.level}"]);
                default:
                    return Resources.Load<Sprite>(GeneralIcons[itemData.id]);
            }
        }
        
        /// <summary>
        /// Max Level IS NOT Indexed (level = 3 is displayed as 3)
        /// </summary>
        public int GetMaxMergeLevel(string id)
        {
            if (ItemInfo.ContainsKey(id))
                return ItemInfo[id].MaxMergeLevel;
            
            CLog.LogError($"[ViewDB] does not contain id {id}");
            return -1;
        }
    }
}