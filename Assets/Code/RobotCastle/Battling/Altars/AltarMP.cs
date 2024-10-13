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
        /// <summary>
        /// Raised every when tier changes. (int prevTier, int newTier)
        /// </summary>
        public event Action<int, int> OnTierUpdated;
    
        public virtual bool IsActive => _tier > 0;

        public virtual int GetTier() => _tier;

        public virtual int SetTier(int tier)
        {
            var prev = _tier;
            _tier = tier;
            if (prev == 0 && tier > 0)
                OnUnlocked?.Invoke();
            else if (prev > 0 && tier == 0)
                OnLocked?.Invoke();
            OnTierUpdated?.Invoke(prev, tier);
            return tier;
        }
        
        public abstract void Apply();
        public abstract string GetShortDescription();
        public abstract string GetDetailedDescription();
        
        [SerializeField] protected string _description;
        [SerializeField] protected string _detailedDescription;
        protected int _tier;
        
        protected void RaiseOnUnlocked() => OnUnlocked?.Invoke();
        
        protected void RaiseOnLocked() => OnLocked?.Invoke();
        
        protected void RaiseOnTierUpdated(int prevTier, int newTier) => OnTierUpdated?.Invoke(prevTier, newTier);

    }

    

}