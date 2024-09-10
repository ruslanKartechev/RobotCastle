using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IUnitBehaviour
    {
        string BehaviourID { get; }
        void Activate(GameObject target, Action<IUnitBehaviour> endCallback);
        
    }
}