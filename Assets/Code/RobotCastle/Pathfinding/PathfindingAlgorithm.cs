using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Bomber
{
    public abstract class PathfindingAlgorithm
    {
        // protected const int MaxNeighbours = 8;
        // protected readonly MapCell[] neighbours = new MapCell[MaxNeighbours];
        protected IMap _map;
        protected HashSet<Vector2Int> closedList;
        protected IBinaryHeap<Vector2Int, PathNode> openList;
        protected IDictionary<Vector2Int, Vector2Int> links;
        protected IList<Vector2Int> pathPoints;

        public PathfindingAlgorithm()
        {
            closedList =  new HashSet<Vector2Int>();
            links = new Dictionary<Vector2Int, Vector2Int>();
            pathPoints = new List<Vector2Int>();
        }
        
        public abstract Task<Path> FindPath(Vector2Int from, Vector2Int target);
        public abstract Task<Path> FindPathOnWaypoints(Vector2Int start, IList<Vector2Int> targets, ICollection<Vector2Int> excluded);
        

        protected void Refresh()
        {
            openList.Clear();
            closedList.Clear();
            links.Clear();
        }
        

    }
}