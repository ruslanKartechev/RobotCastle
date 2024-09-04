namespace RobotCastle.Battling
{
    public interface ITroopsCountView
    {
        void SetCount(int count, int max);
        void UpdateCount(int count, int max);
    }
}