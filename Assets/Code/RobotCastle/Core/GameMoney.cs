using RobotCastle.Saving;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Core
{
    public delegate void MoneyUpdateDelegate(int newVal , int prevVal);
    
    public class GameMoney : MonoBehaviour
    {
        public static GameMoney Create()
        {
            var go = new GameObject("game_money");
            UnityEngine.Object.DontDestroyOnLoad(go);
            var me = go.AddComponent<GameMoney>();
            me._playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            if (me._playerData == null)
            {
                CLog.LogRed("ERROR player data is null ------------");
            }
            return me;
        }
        
        /// <summary>
        /// Passes (newVal, previousValue)
        /// </summary>
        public event MoneyUpdateDelegate OnMoneySet;
        /// <summary>
        /// Passes (newVal, addedValue)
        /// </summary>
        public event MoneyUpdateDelegate OnMoneyAdded;
        
        /// <summary>
        /// Passes (newVal, previousValue)
        /// </summary>
        public event MoneyUpdateDelegate OnGlobalMoneySet;
        /// <summary>
        /// Passes (newVal, addedValue)
        /// </summary>
        public event MoneyUpdateDelegate OnGlobalMoneyAdded;
        
        /// <summary>
        /// Passes (newVal, previousValue)
        /// </summary>
        public event MoneyUpdateDelegate OnGlobalHardMoneySet;
        /// <summary>
        /// Passes (newVal, addedValue)
        /// </summary>
        public event MoneyUpdateDelegate OnGlobalHardMoneyAdded;

        public int levelMoney
        {
            get => _playerData.levelMoney;
            set
            {
                var prev = _playerData.levelMoney;
                _playerData.levelMoney = value;
                OnMoneySet?.Invoke(value, prev);
            }
        }
        
        public int globalMoney
        {
            get => _playerData.globalMoney;
            set
            {
                var prev = _playerData.globalMoney;
                _playerData.globalMoney = value;
                OnGlobalMoneySet?.Invoke(value, prev);
            }
        }

        public int globalHardMoney
        {
            get => _playerData.globalHardMoney;
            set
            {
                var prev = _playerData.globalHardMoney;
                _playerData.globalHardMoney = value;
                OnGlobalHardMoneySet?.Invoke(value, prev);
            }
        }
        
        public SavePlayerData PlayerData
        {
            get => _playerData;
            set => _playerData = value;
        }

        private SavePlayerData _playerData;

        public int AddMoney(int added)
        {
            var prev = _playerData.levelMoney;
            _playerData.levelMoney += added;
            OnMoneyAdded?.Invoke(_playerData.levelMoney, added);
            OnMoneySet?.Invoke(_playerData.levelMoney, prev);
            return _playerData.levelMoney;
        }
        
        public int AddGlobalMoney(int added)
        {
            var prev = _playerData.globalMoney;
            _playerData.globalMoney += added;
            OnGlobalMoneyAdded?.Invoke(_playerData.globalMoney, added);
            OnGlobalMoneySet?.Invoke(_playerData.globalMoney, prev);
            return _playerData.globalMoney;
        }

        public int AddGlobalHardMoney(int added)
        {
            var prev = _playerData.globalHardMoney;
            _playerData.globalHardMoney += added;
            OnGlobalHardMoneyAdded?.Invoke(_playerData.globalHardMoney, added);
            OnGlobalHardMoneySet?.Invoke(_playerData.globalHardMoney, prev);
            return _playerData.globalHardMoney;
        }
    }
    
}