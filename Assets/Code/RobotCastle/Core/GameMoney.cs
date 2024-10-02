using System;
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
        
        public event MoneyUpdateDelegate OnMoneySet;
        public event MoneyUpdateDelegate OnMoneyAdded;
        
        public event MoneyUpdateDelegate OnGlobalMoneySet;
        public event MoneyUpdateDelegate OnGlobalMoneyAdded;
        
        public event MoneyUpdateDelegate OnGlobalHardMoneySet;
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
                OnGlobalMoneySet?.Invoke(value, prev);
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
            OnMoneyAdded?.Invoke(added, prev);
            OnMoneySet?.Invoke(_playerData.levelMoney, prev);
            return _playerData.levelMoney;
        }
        
        public int AddGlobalMoney(int added)
        {
            var prev = _playerData.globalMoney;
            _playerData.globalMoney += added;
            OnMoneyAdded?.Invoke(added, prev);
            OnMoneySet?.Invoke(_playerData.globalMoney, prev);
            return _playerData.globalMoney;
        }

        public int AddGlobalHardMoney(int added)
        {
            var prev = _playerData.globalHardMoney;
            _playerData.globalHardMoney += added;
            OnMoneyAdded?.Invoke(added, prev);
            OnMoneySet?.Invoke(_playerData.globalHardMoney, prev);
            return _playerData.globalHardMoney;
        }
    }
    
}