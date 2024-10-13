using System;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    /// <summary>
    /// Altar Modifier Provider
    /// </summary>
    [System.Serializable]
    public abstract class AltarMP 
    {
        public event Action OnUnlocked;
        public event Action OnLocked;
        public event Action OnTierUpdated;
    
        public virtual bool IsActive => _tier > 0;

        public virtual int GetTier() => _tier;
        
        public abstract int SetTier(int tier);
        public abstract void Apply();
        public abstract string GetShortDescription();
        public abstract string GetDetailedDescription();
        
        [SerializeField] protected string _description;
        [SerializeField] protected string _detailedDescription;
        protected int _tier;
        
        protected void RaiseOnUnlocked() => OnUnlocked?.Invoke();
        
        protected void RaiseOnLocked() => OnLocked?.Invoke();
        
        protected void RaiseOnTierUpdated() => OnTierUpdated?.Invoke();

    }

    

}