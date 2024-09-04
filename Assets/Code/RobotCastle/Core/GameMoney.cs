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
            return data.money;
        }
        
    }
    
}