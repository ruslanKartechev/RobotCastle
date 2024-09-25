using UnityEngine;

namespace RobotCastle.Battling
{
    public class DefaultFullManaAction : IFullManaListener
    {
        public void OnFullMana(GameObject heroGo)
        {
            var stats = heroGo.GetComponent<HeroStatsManager>();
            stats.ManaCurrent.SetBaseAndCurrent(0);
        }
    }
}