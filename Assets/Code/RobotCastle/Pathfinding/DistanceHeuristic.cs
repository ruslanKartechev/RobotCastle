using UnityEngine;

namespace Bomber
{
    public class DistanceHeuristic : IHeuristicFunction
    {
        public float GetHeuristic(Vector2Int start, Vector2Int end)
        {
            return (end - start).DistanceEstimateSum();
        }
    }

    public class DijkstraHeuristic : IHeuristicFunction
    {
        public float GetHeuristic(Vector2Int start, Vector2Int end)
        {
            return 0;
        }
    }
}