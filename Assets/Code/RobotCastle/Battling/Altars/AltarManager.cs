using RobotCastle.Core;
using RobotCastle.Data;

namespace RobotCastle.Battling.Altars
{
    public class AltarManager
    {
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
            var saves = DataHelpers.GetAltarsSave();
            for (var i = 0; i < saves.altars.Count; i++)
            {
                var save = saves.altars[i];
                _db.GetAltar(i).SetPoints(save.points);
            }
        }

        public bool AddPointToAltar(Altar altar)
        {
            var index = _db.GetIndexOf(altar);
            var save = _save.altars[index];
            if (save.points >= Altar.MaxPoints)
                return false;
            save.points++;
            altar.AddPoint();
            return true;
        }

        public void AddPointsToAltar(Altar altar, int points)
        {
            var index = _db.GetIndexOf(altar);
            var save = _save.altars[index];
            var canAdd = save.points < Altar.MaxPoints
                         && _save.pointsFree > 0;

            for (var i = 0; i < points && canAdd; i++)
            {
                save.points++;
                altar.AddPoint();
                canAdd = save.points < Altar.MaxPoints
                    && _save.pointsFree > 0;
            }
        }

        public void RemoveAllPointsFromAllAltars()
        {
            for (var i = 0; i < _save.altars.Count; i++)
            {
                var save = _save.altars[i];
                save.points = 0;
                _db.GetAltar(i).SetPoints(0);
            }
            _save.pointsFree = _save.pointsTotal;
        }
        
        public void RemoveAllPointFromAltar(Altar altar)
        {
            var index = _db.GetIndexOf(altar);
            var save = _save.altars[index];
            var p = save.points;
            _save.pointsFree += p;
            save.points = 0;
            altar.SetPoints(0);
        }

        public bool TryPurchaseOneFreePoint()
        {
            var player = DataHelpers.GetPlayerData();
            if (_db.HasReachedMaxPoints(player.altars.pointsTotal))
                return false;
            var cost = (int)_db.GetNextPointCost(player.playerLevel + 1);
            if(cost == 0) return false; // max level reached case

            var money = player.globalMoney;
            if (money < cost)
                return false;
            money -= cost;
            player.globalMoney = money;
            player.altars.pointsTotal++;
            player.altars.pointsFree++;
            return true;
        }
        
    }
}