namespace RobotCastle.Battling
{
    public interface IDamageDecorator
    {
        string id { get; }
        DamageArgs Apply(DamageArgs damageArgs);
    }
}