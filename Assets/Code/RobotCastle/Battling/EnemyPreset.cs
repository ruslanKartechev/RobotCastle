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
    }
}