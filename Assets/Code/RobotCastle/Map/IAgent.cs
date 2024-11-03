using UnityEngine;

namespace Bomber
{
    public interface IAgent
    {
        bool IsMoving { get; }
        Vector2Int CurrentCell { get; }
        Vector2Int TargetCell { get; }
        string name { get; }
        
    }
}