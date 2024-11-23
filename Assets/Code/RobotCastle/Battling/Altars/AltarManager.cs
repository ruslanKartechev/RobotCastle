using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Battling.Altars
{
    public class AltarManager
    {
        public event ValueUpdate<int> OnFreePointsCountChanged;
        
        private AltarsDatabase _db;
        
        private AltarsSave _save;

        public AltarManager()
        {
            _db = ServiceLocator.Get<AltarsDatabase>();
            _save = DataHelpers.GetAltarsSave();
        }
        
        /// <summary>
        /// Loads level points from save data and initializes altars' tiers
        /// </summary>
        public void SetupData()
        {
            CLog.Log($"[{nameof(AltarManager)}] SetupData");

            var saves = DataHelpers.GetAltarsSave();
            for (var i = 0; i < saves.altars.Count; i++)
            {
                var save = saves.altars[i];
                _db.GetAltar(i).SetPoints(save.points);
            }
        }

        public void SetupAndApplyAll()
        {
            CLog.Log($"[{nameof(AltarManager)}] ApplyAll");
            var saves = DataHelpers.GetAltarsSave();
            for (var i = 0; i < saves.altars.Count; i++)
            {
                var save = saves.altars[i];
                _db.GetAltar(i).SetPoints(save.points);
                if (save.points > 0)
                {
                    var mods = _db.GetAltar(i).modifiers;
                    foreach (var mod in mods)
                        mod.Apply();
                }
            }
        }

        /// <returns>True if added, false if didn't </returns>
        public bool AddPointToAltar(Altar altar)
        {
            if (_save.pointsFree < 1)
                return false;
            var index = _db.GetIndexOf(altar);
            var save = _save.altars[index];
            if (save.points >= Altar.MaxPoints)
                return false;
            var prevFree = _save.pointsFree;
            save.points++;
            _save.pointsFree--;
            altar.SetPoints(save.points);
            SleepDev.Analytics.OnAltarPointAdded( altar.Id, save.points);
            OnFreePointsCountChanged?.Invoke(prevFree, _save.pointsFree);
            return true;
        }


        /// <returns>True if added at least 1 point</returns>
        public bool AddPointsToAltar(Altar altar, int points)
        {
            if (_save.pointsFree < 1)
                return false;
            var index = _db.GetIndexOf(altar);
            var save = _save.altars[index];
            var canAdd = save.points < Altar.MaxPoints
                         && _save.pointsFree > 0;
            if (!canAdd)
                return false;
            var prevFree = _save.pointsFree;
            for (var i = 0; i < points && canAdd; i++)
            {
                save.points++;
                _save.pointsFree--;
                canAdd = save.points < Altar.MaxPoints
                    && _save.pointsFree > 0;
            }
            altar.SetPoints(save.points);
            SleepDev.Analytics.OnAltarPointAdded(altar.Id, save.points);
            OnFreePointsCountChanged?.Invoke(prevFree, _save.pointsFree);
            return true;
        }

        public void RemoveAllPointsFromAllAltars()
        {
            var prevFree = _save.pointsFree;
            for (var i = 0; i < _save.altars.Count; i++)
            {
                var save = _save.altars[i];
                save.points = 0;
                _db.GetAltar(i).SetPoints(0);
            }
            _save.pointsFree = _save.pointsTotal;
            SleepDev.Analytics.AltarPointsReset(_save.pointsFree);
            OnFreePointsCountChanged?.Invoke(prevFree, _save.pointsFree);
        }
        
        public void RemoveAllPointFromAltar(Altar altar)
        {
            var prevFree = _save.pointsFree;
            var index = _db.GetIndexOf(altar);
            var save = _save.altars[index];
            var p = save.points;
            _save.pointsFree += p;
            save.points = 0;
            altar.SetPoints(0);
            OnFreePointsCountChanged?.Invoke(prevFree, _save.pointsFree);
        }

        public int GetPointCost()
        {
            var player = DataHelpers.GetPlayerData();
            return (int)_db.GetNextPointCost(player.playerLevel + 1);
        }
        
        public int GetLevel()
        {
            var player = DataHelpers.GetPlayerData();
            return player.playerLevel + 1;
        }

        /// <summary>
        /// </summary>
        /// <returns>0 - can, 1 - not enough money, 2 - level not met, 3 - max points</returns>
        public int CanBuyOnFreePoint()
        {
            var player = DataHelpers.GetPlayerData();
            if (player.playerLevel < 1)
                return 2;
            if (_db.HasReachedMaxPoints(player.altars.pointsTotal))
                return 3;
            var lvl = player.playerLevel + 1;
            var cost = (int)_db.GetNextPointCost(lvl);
            if(cost == 0) return 3; // max level reached case
            
            var money = ServiceLocator.Get<GameMoney>().globalMoney.Val;
            if (money < cost)
                return 1;
            if (_db.CanBuyMorePoints(player.altars.pointsTotal, lvl) == false)
                return 2;
            return 0;
        }
        
        public bool TryPurchaseOneFreePoint()
        {
            var player = DataHelpers.GetPlayerData();
            if (_db.HasReachedMaxPoints(player.altars.pointsTotal))
                return false;
            var cost = (int)_db.GetNextPointCost(player.playerLevel + 1);
            if(cost == 0) return false; // max level reached case

            var money = ServiceLocator.Get<GameMoney>().globalMoney.Val;
            if (money < cost)
                return false;
            var prevFree = _save.pointsFree;
            money -= cost;
            player.globalMoney = money;
            player.altars.pointsTotal++;
            player.altars.pointsFree++;
            SleepDev.Analytics.AltarPointsPurchased(_save.pointsFree);
            OnFreePointsCountChanged?.Invoke(prevFree, _save.pointsFree);
            return true;
        }
        
    }
}