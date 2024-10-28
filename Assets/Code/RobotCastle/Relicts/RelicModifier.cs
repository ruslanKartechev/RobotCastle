using UnityEngine;

namespace RobotCastle.Relicts
{
    public abstract class RelicModifier : ScriptableObject
    {
        public abstract string GetFullDescription();
        public abstract void Apply();
        
    }
}