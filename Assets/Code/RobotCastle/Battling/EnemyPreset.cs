using System.Collections.Generic;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class EnemyPreset
    {
        /// <summary>
        /// Hero level (1-20). Used to assign hero stats
        /// </summary>
        public int heroLevel;
        public CoreItemData enemy;
        public Vector2Int gridPos;
        public List<CoreItemData> items;
        public List<string> modifiers;
        public EUnitType unitType = EUnitType.Default;
        public bool canDropItems;
        public bool giveRandomItem;

        public EnemyPreset()
        {
            heroLevel = 0;
            enemy = new CoreItemData(0, "", "unit");
            gridPos = default;
            items = new ();
            modifiers = new ();
            canDropItems = true;
            giveRandomItem = false;
        }

        public EnemyPreset(EnemyPreset other)
        {
            heroLevel = other.heroLevel;
            enemy = new CoreItemData(other.enemy);
            gridPos = other.gridPos;
            var count = other.items.Count;
            items = new(count);
            for (var i = 0; i < count; i++)
                items.Add(new CoreItemData(other.items[i]));
            count = other.modifiers.Count;
            modifiers = new(count);
            for(var i = 0; i < count; i++)
                modifiers.Add(other.modifiers[i]);
            unitType = other.unitType;
            canDropItems = other.canDropItems;
            giveRandomItem = other.giveRandomItem;
        }
    }
}