﻿using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class DifficultyTier
    {
        public int totalPower;
        public float multiplier = 1;
        public List<CoreItemData> additionalRewards;
    }
}