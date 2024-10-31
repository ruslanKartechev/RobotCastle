using UnityEngine;

namespace RobotCastle.Relics
{
    public abstract class RelicModifier : ScriptableObject
    {
        public abstract string GetFullDescription();
        public abstract void Apply();
        
    }
}