using RobotCastle.Battling;
using UnityEngine;

namespace RobotCastle.UI
{
    public interface IDamageDisplay
    {
        void ShowAt(int amount, EDamageType type, Vector3 worldPosition);
        void ShowAtScreenPos(int amount, EDamageType type, Vector3 screenPos);
        
        void ShowMightyBlock(Vector3 worldPosition);
        
        void ShowVampirism(int amount, Vector3 worldPosition);
        
    }
}