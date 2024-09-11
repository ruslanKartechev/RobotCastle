using UnityEngine;

namespace RobotCastle.Battling
{
    public abstract class Spell : ScriptableObject
    {
        public abstract string GetName();
        public abstract Sprite GetIcon();
        
        public abstract void AddTo(GameObject hero);
        public abstract string GetDescription(GameObject hero);
    }
}