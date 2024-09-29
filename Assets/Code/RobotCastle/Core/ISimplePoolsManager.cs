namespace RobotCastle.Core
{
    public interface ISimplePoolsManager
    {
        IPoolItem GetOne(string type);
        void ReturnOne(IPoolItem obj);
        bool HasPool(string id);
        void AddPoolIfNot(string id, string prefabPath, int startCount);
    }
}