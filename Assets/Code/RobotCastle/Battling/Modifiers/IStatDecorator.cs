namespace RobotCastle.Battling
{
    public interface IStatDecorator
    {
        string name { get; }
        int order { get; }
        float Decorate(float val);
    }
}