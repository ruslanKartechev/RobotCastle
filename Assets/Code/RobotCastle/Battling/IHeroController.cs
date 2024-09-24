namespace RobotCastle.Battling
{
    public interface IHeroController
    {
        bool IsDead { get; }
        int TeamNum { get; set; }
        Battle Battle { get; set; }
        HeroView View { get; }
        void InitHero(string id, int heroLevel, int mergeLevel);
        void PrepareForBattle();
        void ResetForMerge();
        // REPLACE THIS
        void SetIdle();
        
        void SetBehaviour(IHeroBehaviour behaviour);
        void StopCurrentBehaviour();
        void MarkDead();
    }
}