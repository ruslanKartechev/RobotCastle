using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Relicts
{
    public class RelicsManager
    {
        public static Sprite GetIconForRelic(string id)
        {
            var icon = ServiceLocator.Get<RelicsDataBase>().relicData[id].icon;
            return Resources.Load<Sprite>(icon);
        }
        
        public static RelicStatModifier GetStatModFromRelic(RelicData relicData)
        {
            if (relicData.modifiers.Count > 0)
            {
                var modName = relicData.modifiers[0];
                var mod = GetModifier(modName) as RelicStatModifier;
                return mod;
            }
            else
            {
                Debug.LogError($"Relic has no modifiers");
            }
            return null;
        }
        
        public static RelicModifier GetModifier(string mod)
        {
            return Resources.Load<RelicModifier>($"config/relics/{mod}");
        }

        public static void TryAddSlotsOnNewLevel(RelicsInventorySave data, int newPlayerLevel)
        {
            CLog.Log($"[TryAddSlotsOnNewLevel] slots {data.unlockedSlotsCount}. NewLevel: {newPlayerLevel}");
            if (data.unlockedSlotsCount >= 3) return;
            var minLevel = ServiceLocator.Get<RelicsDataBase>().playerLevelsToUnlockSlots[data.unlockedSlotsCount];
            if (newPlayerLevel >= minLevel)
                data.unlockedSlotsCount++;
        }

        public static void ApplyAllSelectedRelics()
        {
            var data = DataHelpers.GetPlayerData().relics;
            var equipped = new List<RelicData>(3);
            var db = ServiceLocator.Get<RelicsDataBase>();
            if (data.allRelics.Count == 0)
            {
                CLog.Log($"No relics in inventory");
                return;
            }
            
            foreach (var ss in data.allRelics)
            {
                if (ss.isEquipped)
                {
                    equipped.Add(db.relicData[ss.core.id]);
                }
            }

            if (equipped.Count == 0)
            {
                CLog.Log($"No relics equipped. {data.allRelics.Count} in inventory");
                return;
            }
            
            var mods = new List<RelicModifier>(6);
            foreach (var relic in equipped)
            {
                foreach (var str in relic.modifiers)
                {
                    var mod = GetModifier(str);
                    if(mod != null)
                        mods.Add(mod);
                }
            }
            foreach (var mod in mods)
                mod.Apply();
        }

        public static void AddRelic(RelicsInventorySave save, string id)
        {
            var item = save.allRelics.Find(t => t.core.id == id);
            if (item != null)
            {
                item.amount++;
            }
            else
            {
                var db = ServiceLocator.Get<RelicsDataBase>();
                save.allRelics.Add(new RelicSave(db.relicData[id].core));
            }
            
        }
        
    }
}