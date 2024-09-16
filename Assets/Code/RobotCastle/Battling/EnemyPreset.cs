using System.Collections.Generic;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class EnemyPreset
    {
        public int heroLevel;
        public CoreItemData enemy;
        public Vector2Int gridPos;
        public List<CoreItemData> items;
    }
}