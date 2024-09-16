using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IUnitBehaviour
    {
        string BehaviourID { get; }
        void Activate(GameObject target, Action<IUnitBehaviour> endCallback);
        void Activate(HeroController hero, Action<IUnitBehaviour> endCallback);

    }
}