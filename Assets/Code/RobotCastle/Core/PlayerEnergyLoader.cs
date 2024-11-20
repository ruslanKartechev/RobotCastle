using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Core
{
    public class PlayerEnergyLoader : MonoBehaviour, IGameLoader
    {
        [SerializeField] private int _energyMin = 60;
        [SerializeField] private int _energyAdded = 40;
        
        
        public void Load() 
        {
            var save = DataHelpers.GetPlayerData();
            if (save.playerEnergy < _energyMin)
            {
                save.playerEnergy += save.playerEnergyMax >= _energyAdded ? save.playerEnergyMax : _energyAdded;
            }
        }
    }
}