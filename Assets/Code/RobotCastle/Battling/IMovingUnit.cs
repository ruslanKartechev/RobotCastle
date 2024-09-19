using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IMovingUnit
    {
        Vector2Int TargetCell { get; set; }
    }
}