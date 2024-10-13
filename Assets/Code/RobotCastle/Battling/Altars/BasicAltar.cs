using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Basic Altar", fileName = "BasicAltar", order = 0)]
    public class BasicAltar : Altar
    {
        protected AltarMP GetModifier(int level)
        {
            return _modifiers[level];
        }

        public override void AddPoint()
        {
            if (_points == MaxPoints) return;
            _points++;
            SetPoints(_points + 1);
        }

        public override void RemovePoints()
        {
            if (_points == 0) return;
            SetPoints(_points - 1);
        }
        
        public override void SetPoints(int points)
        {
            _points = points;
            if (points == 0)
            {
                foreach (var mod in _modifiers)
                    mod.SetTier(0);
            }
            else
            {
                var pointsLeft = points;
                const int max = 5;
                var msg = $"[SetPoints] Altar: {ViewName}";
                for (var modId = 0; modId < _modifiers.Count; modId++)
                {
                    // var modTier = pointsLeft > max ? max : pointsLeft;
                    pointsLeft -= max;
                    var mod = _modifiers[modId];
                    msg += $"i {modId}, tier: {pointsLeft}";
                    mod.SetTier(pointsLeft);
                }
                CLog.Log(msg);
            }
        }
    }
    
}