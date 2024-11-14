using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public interface IHeroController
    {
        bool IsDead { get; set; }
        int TeamNum { get; set; }
        Battle Battle { get; set; }
        HeroComponents Components { get; }

        IBehaviourProvider DefaultBehaviourProvider { get; set; }
        void SetDefaultBehaviour();
        
        void InitHero(string id, int heroLevel, int mergeLevel, List<ModifierProvider> spells);
        void SetBehaviour(IHeroBehaviour behaviour);
        void StopCurrentBehaviour();
        void MarkDead();
        
        void PauseCurrentBehaviour();
        void ResumeCurrentBehaviour();
    }
}