using RobotCastle.Saving;
using SleepDev;
using SleepDev.Data;
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
            me._playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            if (me._playerData == null)
            {
                CLog.LogRed("ERROR player data is null ------------");
                me.levelMoney = new ReactiveInt();
                me.globalMoney = new ReactiveInt();
                me.globalHardMoney = new ReactiveInt();
                return me;
            }
            else
            {
                me.levelMoney = new ReactiveInt(me._playerData.levelMoney);
                me.globalMoney = new ReactiveInt(me._playerData.globalMoney);
                me.globalHardMoney = new ReactiveInt(me._playerData.globalHardMoney);
            }
            
            return me;
        }

        public ReactiveInt levelMoney;
        
        public ReactiveInt globalMoney;
        
        public ReactiveInt globalHardMoney;

        public void SyncSaves()
        {
            _playerData.levelMoney = levelMoney.Val;
            _playerData.globalMoney = globalMoney.Val;
            _playerData.globalHardMoney = globalHardMoney.Val;
        }
        
        public SavePlayerData playerData => _playerData;
        
        private SavePlayerData _playerData;

        public int AddMoney(int added) => levelMoney.AddValue(added);
 
        public int AddGlobalMoney(int added) => globalMoney.AddValue(added);
        
        public int AddGlobalHardMoney(int added) => globalHardMoney.AddValue(added);
    }
    
}