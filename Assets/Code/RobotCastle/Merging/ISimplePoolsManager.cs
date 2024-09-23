using RobotCastle.Core;

namespace RobotCastle.Merging
{
    public interface ISimplePoolsManager
    {
        IPoolItem GetOne(string type);
        void ReturnOne(IPoolItem obj);
        bool HasPool(string id);
        void AddPoolIfNot(string id, string prefabPath, int startCount);

    }
}