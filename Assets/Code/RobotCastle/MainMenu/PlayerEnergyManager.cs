using RobotCastle.Core;
using RobotCastle.Saving;
using SleepDev;
using SleepDev.Data;

namespace RobotCastle.MainMenu
{
    public class PlayerEnergyManager 
    {
        public static PlayerEnergyManager Create()
        {
            var me = new PlayerEnergyManager();
            me._playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            if (me._playerData == null)
            {
                CLog.LogRed("ERROR player data is null ------------");
            }
            return me;
        }

        /// <summary>
        /// Passes current and max
        /// </summary>
        public event UpdateDelegate<int> OnEnergySet;
        public const int PlayLevelEnergyCost = 5;
        
        private SavePlayerData _playerData;
        
        public int GetCurrent()
        {
            return _playerData.playerEnergy;
        }

        public int GetMax()
        {
            return _playerData.playerEnergyMax;
        }
        
        public string GetAsStr() => $"{GetCurrent()}/{GetMax()}";

        public void Set(int energy)
        {
            var prev = _playerData.playerEnergy;
            _playerData.playerEnergy = energy;
            OnEnergySet?.Invoke(energy, prev);
        }

        public void Subtract()
        {
            Subtract(PlayLevelEnergyCost);
        }

        public void Subtract(int amount)
        {
            var prev = _playerData.playerEnergy;
            _playerData.playerEnergy = prev - amount;
            OnEnergySet?.Invoke(_playerData.playerEnergy, prev);
        }
        
        private PlayerEnergyManager(){}
        
    }
}