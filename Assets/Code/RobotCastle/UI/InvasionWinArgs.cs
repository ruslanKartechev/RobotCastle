using System;
using System.Collections.Generic;
using RobotCastle.Data;
using RobotCastle.InvasionMode;

namespace RobotCastle.UI
{
    public class InvasionWinArgs
    {
        public bool playerNewLevelReached;
        public float playerXpAdded;
        public List<float> heroesXp = new(6);
        public List<CoreItemData> rewards = new();
        public Action returnCallback;
        public Action replayCallback;
        public ChapterSelectionData selectionData;
    }
}