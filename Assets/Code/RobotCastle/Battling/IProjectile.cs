using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IProjectile
    {
        void LaunchFrom(Transform startPoint, Transform endPoint, float speed, Action<object> hitCallback, object target);
    }
}