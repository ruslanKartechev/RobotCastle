using System;

namespace SleepDev
{
    /// <summary>
    /// T is the type on which the command is supposed to be executed
    /// </summary>
    public interface ICommand<T>
    {
        void Execute(T target, Action onCompleted);
    }
}