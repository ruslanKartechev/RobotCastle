using UnityEngine;

namespace RobotCastle.Core
{
    public interface IPoolItem
    {
        GameObject GetGameObject();
        string PoolId { get; set; }
        void PoolHide();
        void PoolShow();
    }
}