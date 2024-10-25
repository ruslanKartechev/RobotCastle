using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    public static class NeighbourFiller
    {
        private static readonly (Vector2Int position, float cost)[] CloseNeighbours = {
            (new Vector2Int(1, 0), 1),
            (new Vector2Int(0, 1), 1),
            (new Vector2Int(-1, 0), 1),
            (new Vector2Int(0, -1), 1),
            (new Vector2Int(1, 1), (float)System.Math.Sqrt(2)),
            (new Vector2Int(1, -1), (float)System.Math.Sqrt(2)),
            (new Vector2Int(-1, 1), (float)System.Math.Sqrt(2)),
            (new Vector2Int(-1, -1), (float)System.Math.Sqrt(2))
        };
             
        public static List<PathNode> Fill(PathNode parent, Vector2Int target, IHeuristicFunction heuristicFunction)
        {
            var buffer = new List<PathNode>(8);
            foreach ((var relativePosition, var cost) in CloseNeighbours)
            {
                var nodePosition = relativePosition + parent.Position;
                var traverseDistance = parent.CostSoFar + cost;
                buffer.Add(new PathNode(nodePosition, traverseDistance, heuristicFunction.GetHeuristic(nodePosition, target)));
            }
            return buffer;
        }   
        
        
        public static List<PathNode> Fill(IMap map, PathNode parent, Vector2Int target, IHeuristicFunction heuristicFunction)
        {
            const int count = 8;
            var buffer = new List<PathNode>(count);
            if (parent.Position.x > 0 && parent.Position.x < map.Size.x-1
                && parent.Position.y > 0 && parent.Position.y < map.Size.y-1)
            {
                for (var i = 0; i < count; i++)
                {
                    (var relativePosition, var cost) = CloseNeighbours[i];
                    var nodePosition = relativePosition + parent.Position;
                    var traverseDistance = parent.CostSoFar + cost;
                    buffer.Add(new PathNode(nodePosition, traverseDistance,
                        heuristicFunction.GetHeuristic(nodePosition, target)));
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    (var relativePosition, var cost) = CloseNeighbours[i];
                    var nodePosition = relativePosition + parent.Position;
                    if (nodePosition.x < 0 || nodePosition.x >= map.Size.x ||
                        nodePosition.y < 0 || nodePosition.y >= map.Size.y)
                    {
                        continue;
                    }
                    var traverseDistance = parent.CostSoFar + cost;
                    buffer.Add(new PathNode(nodePosition, traverseDistance, heuristicFunction.GetHeuristic(nodePosition, target)));
                }
            }
            return buffer;
        }   
    }
}