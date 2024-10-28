using System.Collections.Generic;

namespace RobotCastle.Relicts
{
    [System.Serializable]
    public class RelicData
    {
        public RelicCore core;
        public List<string> modifiers;
        public string viewName;
        public string description;
        public string icon;
    }
}