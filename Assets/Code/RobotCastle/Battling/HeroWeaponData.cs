using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroWeaponData
    {
        public CoreItemData core;
        public List<string> modifierIds;

        public string id => core.id;
        public int level => core.level;
        public string type => core.type;

        public static List<HeroWeaponData> GetDataWithDefaultModifiers(List<CoreItemData> items)
        {
            var result = new List<HeroWeaponData>(items.Count);
            var db = ServiceLocator.Get<ViewDataBase>();
            foreach (var it in items)
            {
                var prefab = db.GetMergePrefabAtLevel(it.id, it.level);
                if (prefab.TryGetComponent<ModifiersContainer>(out var container))
                    result.Add(new HeroWeaponData(it, container.ModifierIds));   
                else
                    result.Add(new HeroWeaponData(it, new List<string>()));

            }
            return result;
        }
        
        public static HeroWeaponData GetDataWithDefaultModifiers(CoreItemData it)
        {
            HeroWeaponData result;
            var db = ServiceLocator.Get<ViewDataBase>();
            var prefab = db.GetMergePrefabAtLevel(it.id, it.level);
            if (prefab.TryGetComponent<ModifiersContainer>(out var container))
                result = new HeroWeaponData(it, container.ModifierIds);   
            else
                result = new HeroWeaponData(it, new List<string>());
            return result;
        }

        public HeroWeaponData(CoreItemData core, List<string> modifier)
        {
            this.core = core;
            this.modifierIds = modifier;
        }
        
        public HeroWeaponData(GameObject source)
        {
            var mergeView = source.GetComponent<IItemView>();
            if (mergeView == null)
            {
                CLog.Log($"Merge view is null!!");
                return;
            }
            core = mergeView.itemData.core;
            var modifiersContainer = source.GetComponent<ModifiersContainer>();
            if (modifiersContainer == null)
            {
                modifierIds = new();
                CLog.Log("Modifiers Container is null");
                return;
            }
            modifierIds = modifiersContainer.ModifierIds;
        }
    }
}