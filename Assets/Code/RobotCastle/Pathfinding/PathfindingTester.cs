using SleepDev;
using UnityEngine;

namespace Bomber
{
    public class PathfindingTester : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private Agent _agent;
        [SerializeField] private MapBuilder _mapBuilder;
        [SerializeField] private IntegerGridPlacer _p1;
        [SerializeField] private IntegerGridPlacer _p2;
        
        private PathfinderAStar _pathfinder;

        private void Awake()
        {
        }


        [ContextMenu("Test Move Agent to P2")]
        public void TestMoveAgent()
        {
            if (_agent == null)
            {
                Debug.LogError($"Agent is not assigned");
                return;
            }
            _mapBuilder.InitRuntime();
            var map = _mapBuilder.Map;
            _p2.Round();
            map.GetCellAtPosition(_p2.transform.position, out var cell2Pos, out var cell2);
            _agent.InitAgent(map);
            _agent.MoveToCellAt(cell2Pos);
        }
        
        
        [ContextMenu("Test Find Path")]
        public void TestFindPath()
        {
            _mapBuilder.InitRuntime();
            var map = _mapBuilder.Map;
            _pathfinder = new PathfinderAStar(map);
            _p1.Round();
            _p2.Round();
            map.GetCellAtPosition(_p1.transform.position, out var cell1Pos, out var cell1);
            map.GetCellAtPosition(_p2.transform.position, out var cell2Pos, out var cell2);
            
            var path = _pathfinder.FindPath(cell1Pos, cell2Pos).Result;
            Debug.Log($"Path {path.points.Count} {path.success}");
            if (path.success)
            {
                CLog.LogBlue($"points count: {path.points.Count}");
                foreach (var point in path.points)
                {
                    var gizmoPoint1 = map.Grid[point.x, point.y].worldPosition;
                    var gizmoPoint2 = gizmoPoint1 + Vector3.up * 5;
                    Debug.DrawLine(gizmoPoint1, gizmoPoint2, Color.green, 15f);
                }
            }
        }
        
    #endif    
    }
    
}