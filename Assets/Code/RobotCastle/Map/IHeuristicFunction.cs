using UnityEngine;

namespace Bomber
{
    public interface IHeuristicFunction
    {
        float GetHeuristic(Vector2Int start, Vector2Int end);
    }
}