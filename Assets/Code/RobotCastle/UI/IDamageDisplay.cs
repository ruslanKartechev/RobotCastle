using RobotCastle.Battling;
using UnityEngine;

namespace RobotCastle.UI
{
    public interface IDamageDisplay
    {
        void ShowAt(int amount, EDamageType type, Vector3 worldPosition);
    }
}