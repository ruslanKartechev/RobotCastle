using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    [System.Serializable]
    public class ViewDataBase
    {
        public Dictionary<string, ItemInfo> ItemInfo;

        public GameObject GetMergePrefabAtLevel(string id, int levelIndex)
        {
            if (ItemInfo.ContainsKey(id))
            {
                var path = $"{ItemInfo[id].Prefab}_lvl_{levelIndex + 1}";
                CLog.LogRed($"id {id}. level {levelIndex}. Path {path}");
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