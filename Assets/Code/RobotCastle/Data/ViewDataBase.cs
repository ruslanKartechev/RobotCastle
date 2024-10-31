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

        public static Sprite GetHeroSprite(string spriteId)
        {
            return Resources.Load<Sprite>($"sprites/{spriteId}");
        }

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

        public Sprite GetUnitItemSpriteAtLevel(string id, int levelIndex)
        {
            if (ItemInfo.ContainsKey(id))
            {
                var path = $"{ItemInfo[id].Icon}_lvl_{levelIndex + 1}";
                return Resources.Load<Sprite>(path);
            }
            CLog.LogError($"[ViewDb] does not contain fullId {id}");
            return null;
        }

        public Sprite GetItemSpriteByTypeAndLevel(CoreItemData itemData)
        {
            string path;
            switch (itemData.type)
            {
                case MergeConstants.TypeWeapons:
                    path = $"{ItemInfo[itemData.id].Icon}_lvl_{itemData.level + 1}";
                    return Resources.Load<Sprite>(path);
                case MergeConstants.TypeBonus:
                    path = GeneralIcons[$"{itemData.id}_{itemData.level}"];
                    return Resources.Load<Sprite>(path);
                default:
                    return Resources.Load<Sprite>(GeneralIcons[itemData.id]);
            }
        }
        
        public Sprite GetUnitItemSprite(string id)
        {
            if (ItemInfo.ContainsKey(id))
                return Resources.Load<Sprite>(ItemInfo[id].Icon);
            CLog.LogError($"[ViewDB] does not contain id {id}");
            return null;
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