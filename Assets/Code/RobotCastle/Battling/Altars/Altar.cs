using System.Collections.Generic;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    public abstract class Altar : ScriptableObject
    {
        public const int MaxPoints = 15;
        public event ValueUpdate<int> OnPointsUpdated;
        
        public virtual List<AltarMP> modifiers => _modifiers;

        public string ViewName => _altarName;
        
        public string Id => _id;

        public abstract void SetPoints(int point);
        
        public virtual int GetPoints() => _points;
        
        public abstract void AddPoint();
        
        public abstract void RemovePoints();
        
        [SerializeField] protected string _id;
        [SerializeField] protected string _altarName;
        protected List<AltarMP> _modifiers;
        protected int _points;

        protected void RaisePointsUpdated(int prev, int newVal) => OnPointsUpdated?.Invoke(prev, newVal);
    }
}