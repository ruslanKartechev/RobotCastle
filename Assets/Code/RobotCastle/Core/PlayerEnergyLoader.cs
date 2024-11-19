using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Core
{
    public class PlayerEnergyLoader : MonoBehaviour, IGameLoader
    {
        public void Load() 
        {
            var save = DataHelpers.GetPlayerData();
            if (save.playerEnergy < 80)
            {
                save.playerEnergy += save.playerEnergyMax >= 40 ? save.playerEnergyMax : 40;
            }
        }
    }
}