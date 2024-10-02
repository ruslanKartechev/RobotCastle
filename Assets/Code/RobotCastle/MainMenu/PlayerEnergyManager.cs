using RobotCastle.Core;
using RobotCastle.Saving;
using SleepDev;

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
        public event MoneyUpdateDelegate OnEnergySet;
        
        private SavePlayerData _playerData;
        
        public int GetCurrent()
        {
            return _playerData.playerEnergy;
        }

        public int GetMax()
        {
            return _playerData.playerEnergyMax;
        }

        public void Set(int val)
        {
            _playerData.playerEnergy = val;
            OnEnergySet?.Invoke(val, GetMax());
        }
        
        private PlayerEnergyManager(){}
        
    }
}