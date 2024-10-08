using UnityEngine;

namespace RobotCastle.Battling.SmeltingOffer
{
    [CreateAssetMenu(menuName = "SO/Smelting Config", fileName = "Smelting Config", order = 450)]
    public class SmeltingConfigContainer : ScriptableObject
    {
        public SmeltingConfig config;
    }
}