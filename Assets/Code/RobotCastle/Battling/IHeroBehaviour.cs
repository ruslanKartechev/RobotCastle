using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IHeroBehaviour
    {
        string BehaviourID { get; }
        void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback);
        void Stop();
    }
}