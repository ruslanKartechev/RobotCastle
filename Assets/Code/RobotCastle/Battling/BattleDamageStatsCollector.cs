using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public class BattleDamageStatsCollector : IBattleDamageStatsCollector
    {
        public event System.Action onUpdated;
        public event System.Action onListUpdated;

        public Dictionary<string, PerHeroData> data => _data;

        private readonly Dictionary<string, PerHeroData> _data = new(10);

        public void AddHero(IHeroController hero)
        {
            hero.Components.healthManager.StatsCollector = this;
            hero.Components.damageSource.StatsCollector = this;
            _data.Add(hero.Components.GUID ,new PerHeroData() { id = hero.Components.stats.HeroId });
            onListUpdated?.Invoke();
        }
        
        public void AddHeroes(List<IHeroController> heroes)
        {
            foreach (var hero in heroes)
            {
                hero.Components.healthManager.StatsCollector = this;
                hero.Components.damageSource.StatsCollector = this;
                _data.Add(hero.Components.GUID, new PerHeroData() { id = hero.Components.stats.HeroId });
            }
            onListUpdated?.Invoke();
        }

        public void ResetForNewHeroes(List<IHeroController> heroes)
        {
            _data.Clear();
            foreach (var hero in heroes)
            {
                hero.Components.healthManager.StatsCollector = this;
                hero.Components.damageSource.StatsCollector = this;
                _data.Add(hero.Components.GUID, new PerHeroData(){ id = hero.Components.stats.HeroId });
            }
            onListUpdated?.Invoke();
        }

        public void AddDamageDealt(string guid, EDamageType type, int amount)
        {
            switch (type)
            {
                case EDamageType.Magical:
                    _data[guid].damageDealtMag += amount;
                    break;
                case EDamageType.Physical:
                    _data[guid].damageDealtPhys += amount;
                    break;
            }
            onUpdated?.Invoke();
        }

        public void AddDamageReceived(string guid, EDamageType type, int amount)
        {
            switch (type)
            {
                case EDamageType.Magical:
                    _data[guid].damageReceivedMag += amount;
                    break;
                case EDamageType.Physical:
                    _data[guid].damageReceivedPhys += amount;
                    break;
            }
            onUpdated?.Invoke();
        }

        public void Reset()
        {
            _data.Clear();
            onUpdated?.Invoke();
        }

        public class PerHeroData
        {
            public string id;
            public int damageDealtPhys;
            public int damageDealtMag;
            public int damageReceivedPhys;
            public int damageReceivedMag;

        }
    }
}