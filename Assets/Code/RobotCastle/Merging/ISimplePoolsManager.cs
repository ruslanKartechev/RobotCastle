using RobotCastle.Core;

namespace RobotCastle.Merging
{
    public interface ISimplePoolsManager
    {
        IPoolObject GetOne(string type);
        void ReturnOne(IPoolObject obj);
        bool HasPool(string id);

    }
}