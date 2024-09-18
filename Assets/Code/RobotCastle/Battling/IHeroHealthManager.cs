namespace RobotCastle.Battling
{
    public interface IHeroHealthManager
    {
        void SetDamageable(bool damageable);

        void Reset();
        void SetFullHealth();
    }
}