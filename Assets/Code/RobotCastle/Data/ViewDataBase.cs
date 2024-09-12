using System;
using System.Collections.Generic;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class ViewDataBase
    {
        public Dictionary<string, ItemInfo> ItemInfo = new (5);
        public Dictionary<string, string> StatsIcons = new (5);
        public Dictionary<string, string> SpellsIcons = new (5);

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

        public Sprite GetStatIcon(EStatType statType)
        {
            return GetIconForStat(GetStatId(statType));
        }
        
        public Sprite GetIconForStat(string statId)
        {
            return Resources.Load<Sprite>($"sprites/{StatsIcons[statId]}");
        }
        
        public GameObject GetMergePrefabAtLevel(string id, int levelIndex)
        {
            if (ItemInfo.ContainsKey(id))
            {
                var path = $"{ItemInfo[id].Prefab}_lvl_{levelIndex + 1}";
                // CLog.LogRed($"id {id}. level {levelIndex}. Path {path}");
                return Resources.Load<GameObject>(path);
            }
            CLog.LogError($"DataBase does not contain id {id}");
            return null;
        }
        
        public GameObject GetMergePrefab(string id)
        {
            if (ItemInfo.ContainsKey(id))
            {
                return Resources.Load<GameObject>(ItemInfo[id].Prefab);
            }
            CLog.LogError($"DataBase does not contain id {id}");
            return null;
        }

        public Sprite GetUnitItemSpriteAtLevel(string id, int levelIndex)
        {
            if (ItemInfo.ContainsKey(id))
            {
                var path = $"{ItemInfo[id].Icon}_lvl_{levelIndex + 1}";
                return Resources.Load<Sprite>(path);
            }
            CLog.LogError($"DataBase does not contain fullId {id}");
            return null;
        }
        
        public Sprite GetUnitItemSprite(string id)
        {
            if (ItemInfo.ContainsKey(id))
                return Resources.Load<Sprite>(ItemInfo[id].Icon);
            CLog.LogError($"DataBase does not contain id {id}");
            return null;
        }
        
        public int GetMaxMergeLevel(string id)
        {
            if (ItemInfo.ContainsKey(id))
                return ItemInfo[id].MaxMergeLevel;
            return -1;
        }
    }
}