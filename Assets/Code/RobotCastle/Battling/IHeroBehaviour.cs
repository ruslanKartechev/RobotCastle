using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IHeroBehaviour
    {
        string BehaviourID { get; }
        void Activate(GameObject target, Action<IHeroBehaviour> endCallback);
        void Activate(HeroController hero, Action<IHeroBehaviour> endCallback);
        void Stop();
    }
}