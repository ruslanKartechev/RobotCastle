namespace RobotCastle.Core
{
    public interface IModifiable<T>
    {
        void AddModifier(T mod);
        void RemoveModifier(T mod);
        void ClearAllModifiers();
    }
}