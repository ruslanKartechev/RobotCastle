using System.Collections.Generic;

namespace RobotCastle.Relics
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