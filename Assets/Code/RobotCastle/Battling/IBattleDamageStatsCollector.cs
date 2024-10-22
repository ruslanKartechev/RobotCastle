using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public interface IBattleDamageStatsCollector
    {
        Dictionary<string, BattleDamageStatsCollector.PerHeroData> data { get; }
        void AddHero(IHeroController hero);
        void ResetForNewHeroes(List<IHeroController> heroes);
        
        void AddDamageDealt(string guid, EDamageType type, int amount);
        void AddDamageReceived(string guid, EDamageType type, int amount);
        void Reset();
    }
}