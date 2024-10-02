using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using SleepDev;

namespace RobotCastle.MainMenu
{
    public class CastleXpManager 
    {
        public static CastleXpManager Create()
        {
            var me = new CastleXpManager();
            me._playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            if (me._playerData == null)
            {
                CLog.LogRed("ERROR player data is null ------------");
            }
            me._xpDatabase = ServiceLocator.Get<XpDatabase>();
            if (me._xpDatabase == null)
            {
                CLog.LogRed("ERROR xpDatabase is null ------------");
            }
            return me;
        }
        
        public event MoneyUpdateDelegate OnXpSet;
        public event MoneyUpdateDelegate OnLevelSet;


        private SavePlayerData _playerData;
        private XpDatabase _xpDatabase;
        
        public int GetLevel() => _playerData.playerLevel;

        public int GetXp()
        {
            return _playerData.playerLevel;
        }

        public int GetMaxXp()
        {
            var lvl = GetLevel();
            if (_xpDatabase.castleXpLevels.Count <= lvl)
                return -1;
            return _xpDatabase.castleXpLevels[lvl];
        }

        public float GetProgressToNextLvl()
        {
            var xp = GetXp();
            var max = GetMaxXp();
            if (max == -1)
                return -1;
            return (float)xp / max;
        }

        public bool AddXp(int added)
        {
            var prev = _playerData.playerXp;
            _playerData.playerXp += added;
            if (_playerData.playerXp >= GetMaxXp())
            {
                _playerData.playerLevel++;
                OnLevelSet?.Invoke(_playerData.playerLevel, _playerData.playerLevel - 1);
                OnXpSet?.Invoke(prev, _playerData.playerXp);
                return true;
            }
            OnXpSet?.Invoke(prev, _playerData.playerXp);
            return false;
        }
        
        private CastleXpManager(){}
        
    }
}