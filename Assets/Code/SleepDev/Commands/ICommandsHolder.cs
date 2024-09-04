namespace SleepDev
{
    /// <summary>
    /// T is the type on which the command is supposed to be executed
    /// </summary>
    public interface ICommandsHolder<T>
    {
        int Count { get; }
        void Clear();
        void Add(ICommand<T> command);
        ICommand<T> GetLast();
        ICommand<T> GetFirst();
    }
}