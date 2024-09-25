namespace RobotCastle.Battling
{
    public interface IStatDecorator
    {
        string name { get; }
        int priority { get; }
        float Decorate(float val);
    }
}