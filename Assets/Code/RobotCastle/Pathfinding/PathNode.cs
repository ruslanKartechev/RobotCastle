using UnityEngine;

namespace Bomber
{
    public readonly struct PathNode
    {
        public PathNode(Vector2Int position, float costSoFar, float heuristic)
        {
            Position = position;
            CostSoFar = costSoFar;
            EstimatedTotalCost = costSoFar + heuristic;
        }

        public Vector2Int Position { get; }
        public float CostSoFar { get; }
        public float EstimatedTotalCost { get; }
    }
}