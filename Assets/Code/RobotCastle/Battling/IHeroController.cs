using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public interface IHeroController
    {
        bool IsDead { get; set; }
        int TeamNum { get; set; }
        Battle Battle { get; set; }
        HeroComponents Components { get; }

        void InitHero(string id, int heroLevel, int mergeLevel, List<ModifierProvider> spells);
        void SetBehaviour(IHeroBehaviour behaviour);
        void StopCurrentBehaviour();
        void MarkDead();
    }
}