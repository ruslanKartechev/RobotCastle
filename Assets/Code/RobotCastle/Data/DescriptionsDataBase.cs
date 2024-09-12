﻿using System.Collections.Generic;

namespace RobotCastle.Merging
{
    [System.Serializable]
    public class DescriptionsDataBase
    {
        public Dictionary<string, DescriptionInfo> descriptions;

        public DescriptionInfo GetDescription(string id)
        {
            return descriptions[id];
        }

    }
}