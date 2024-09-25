using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public abstract class ModifierProvider : ScriptableObject
    {
        public static ModifierProvider LoadModifier(string id)
        {
            return Resources.Load<ModifierProvider>($"config/Modifiers/{id}");
        }
     
        
        [SerializeField] protected string _id;

        public virtual string GetId() => _id;
        
        /// <summary>
        /// Adds the spell, stat modifier, etc., to given target (hero) 
        /// </summary>
        public abstract void AddTo(GameObject target);
        public abstract void AddToHero(HeroView view);

        /// <summary>
        /// Gives a detailed description based on all applied modifiers.
        /// Since they can affect each other, may require the "target" to obtain full information 
        /// </summary>
        public abstract string GetDescription(GameObject target);

    }
}