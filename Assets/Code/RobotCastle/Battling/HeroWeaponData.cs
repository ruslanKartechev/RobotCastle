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