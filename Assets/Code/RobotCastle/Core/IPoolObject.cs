using UnityEngine;

namespace RobotCastle.Core
{
    public interface IPoolObject
    {
        GameObject GetGameObject();
        string PoolId { get; set; }
        void PoolHide();
        void PoolShow();
    }
}