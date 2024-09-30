using System;
using RobotCastle.Saving;
using UnityEngine;

namespace RobotCastle.Core
{
    public class GameMoney : MonoBehaviour
    {
        public static GameMoney Create()
        {
            var go = new GameObject("game_money");
            UnityEngine.Object.DontDestroyOnLoad(go);
            var me = go.AddComponent<GameMoney>();
            return me;
        }
        
        public event Action<int> OnMoneyUpdated;
        /// <summary>
        /// Pass the added amount. Used for animated money added calls
        /// </summary>
        public event Action<int> OnMoneyAdded;

        public int Money
        {
            get
            {
                var data = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
                return data.money;
            }
            set
            {
                var data = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
                data.money = value;
                OnMoneyUpdated?.Invoke(value);
            }
        }

        public int AddMoney(int added)
        {
            var data = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            data.money += added;
            OnMoneyUpdated?.Invoke(data.money);
            OnMoneyAdded?.Invoke(added);
            return data.money;
        }
        
    }
    
}