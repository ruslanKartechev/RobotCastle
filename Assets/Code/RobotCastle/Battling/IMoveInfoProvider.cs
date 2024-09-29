using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IMoveInfoProvider
    {
        Vector2Int TargetCell { get; set; }
    }
}