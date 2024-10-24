using System;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public abstract class Tab : MonoBehaviour
    {
        public abstract void Show(Action closeCallback);
        public abstract void Close();
    }
}