namespace RobotCastle.Battling
{
    public interface IAttackAction
    {
        void Attack(IDamageReceiver target, int animationIndex);
    }
}