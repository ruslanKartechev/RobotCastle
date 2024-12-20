﻿#define DRAW_STEP_BY_STEP__
#define DRAW_STEP_BY_STEP_DELAY__

using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Bomber
{
    public class PathfinderAStar : PathfindingAlgorithm
    {
        private const int IterationsMaxCount = 300;
        private Vector2Int _currentEndPoint;
        private IHeuristicFunction _heuristicFunction;
        private HashSet<Vector2Int> _occupied;
        private ISet<Vector2Int> _excludedPos;

        private int _stepsPerFrameMax = 100; 
        
        public int StepsPerFrameMax
        {
            get => _stepsPerFrameMax;
            set
            {
                _stepsPerFrameMax = value;
                if (_stepsPerFrameMax < 2)
                    _stepsPerFrameMax = 2;
            } 
        }
        
        public IMap Map
        {
            get => _map;
            set => _map = value;
        }

        public PathfinderAStar(IMap map, IHeuristicFunction heuristicFunction = null) : base()
        {
            Map = map;
            if (heuristicFunction == null)
                _heuristicFunction = new DistanceHeuristic();
            else
                _heuristicFunction = heuristicFunction;
            _excludedPos = new HashSet<Vector2Int>();
            
            openList = new BinaryHeapNodes(new MapCellsComparer());
        }

        public override async Task<Path> FindPath(Vector2Int start, Vector2Int target)
        {
            Refresh();
            pathPoints.Clear();
            var foundPathToDestination = await GeneratePath(start, target);
            var points = new List<Vector2Int>(24);
            points.Add(_currentEndPoint);
            while (links.TryGetValue(_currentEndPoint, out _currentEndPoint))
                points.Add(_currentEndPoint);
            pathPoints = new List<Vector2Int>(points.Count);
            // reversing
            for ( var i = points.Count - 1; i >= 0; i--)
                pathPoints.Add(points[i]);
            // RemoveExtraPoints(pathGridPoints, _map);
            return new Path(pathPoints, foundPathToDestination);
        }
        
        public override async Task<Path> FindPathOnWaypoints(Vector2Int start, IList<Vector2Int> targets, ICollection<Vector2Int> excluded)
        {
            Refresh();
            var foundPathToTarget = true;
            var count = targets.Count;
            var pathFragments = new List<List<Vector2Int>>(count);
            foreach (var pos in excluded)
                _excludedPos.Add(pos);
            for (var i = 0; i < count; i++)
            {
                var ithDest = targets[i];
                var pathFragment = new List<Vector2Int>();
                var found = await GeneratePath(start, ithDest);
                closedList.Clear();
                pathFragment.Add(_currentEndPoint);
                while (links.TryGetValue(_currentEndPoint, out _currentEndPoint))
                {
                    pathFragment.Add(_currentEndPoint);
                    closedList.Add(_currentEndPoint);
                }
                if (i != count - 1)
                    pathFragment.Remove(ithDest);
                if (!found)
                {
                    foundPathToTarget = false;
                    break;
                }
                start = ithDest;
                openList.Clear();
                links.Clear();
                // RemoveExtraPoints(pathFragment, _map);
                pathFragments.Add(pathFragment);

            }
            var points = new List<Vector2Int>(24);
            points.Add(_currentEndPoint);
            for (var i = pathFragments.Count - 1; i >= 0; i--)
            {
                foreach (var coord in pathFragments[i])
                    points.Add(coord);
            }
            // reversing
            for ( var i = points.Count - 1; i >= 0; i--)
                pathPoints.Add(points[i]);
            return new Path(pathPoints, foundPathToTarget);
        }
        
        
        public async Task<Path> FindPath(Vector2Int start, Vector2Int target, ICollection<Vector2Int> excluded)
        {
            Refresh();
            foreach (var pos in excluded)
                _excludedPos.Add(pos);
            var foundPathToDestination = await GeneratePath(start, target);
            pathPoints.Add(_currentEndPoint);
            while (links.TryGetValue(_currentEndPoint, out _currentEndPoint))
                pathPoints.Add(_currentEndPoint);
            // RemoveExtraPoints(pathGridPoints, _map);
            return new Path(pathPoints, foundPathToDestination);
        }

        private async Task<bool> GeneratePath(Vector2Int start, Vector2Int target)
        {
            var currentNode = new PathNode(start, 0, _heuristicFunction.GetHeuristic(start, target));
            openList.Enqueue(currentNode);
            var steps = 0;
            var stepsBeforeAwait = 0;
            while (openList.Count > 0 && steps < IterationsMaxCount)
            {
                currentNode = openList.Dequeue();
                closedList.Add(currentNode.Position);
                _currentEndPoint = currentNode.Position;
                if (_currentEndPoint == target)
                    return true;
                var neighbours = NeighbourFiller.Fill(_map, currentNode, target, _heuristicFunction);
                foreach (var nextNode in neighbours)
                {
                    var nextPos = nextNode.Position;
                    if (closedList.Contains(nextPos)) 
                        continue;
                    if (_map.Grid[nextPos.x, nextPos.y].isPlayerWalkable == false)
                        continue;
                    
                    if (nextPos != target)
                    {
                        var otherAgentOccupied = false;
                        foreach (var agent in _map.ActiveAgents)
                        {
                            if (agent.CurrentCell == nextPos)
                            {
                                otherAgentOccupied = true;
                                break;
                            }
                        }
                        if (otherAgentOccupied)
                            continue;
                    }
                    
#if DRAW_STEP_BY_STEP
                    var drawPos1 = _map.Grid[nextPos.x, nextPos.y].worldPosition;
                    var drawPos2 = drawPos1 + Vector3.up;
                    var drawColor = Color.white;
                    var drawTime = 1f;
#endif
                    if (openList.TryGet(nextPos, out var existingNode) == false) // new
                    {
                        openList.Enqueue(nextNode);
                        links[nextPos] = currentNode.Position; // Add link to dictionary.
#if DRAW_STEP_BY_STEP
                        drawColor = Color.blue;
#endif
                    }
                    else
                    {
#if DRAW_STEP_BY_STEP
                        drawColor = Color.white;
#endif
                        if (nextNode.EstimatedTotalCost < existingNode.EstimatedTotalCost) // checked
                        {
                            openList.Modify(nextNode);
                            links[nextPos] = currentNode.Position; // Add link to dictionary.
#if DRAW_STEP_BY_STEP
                            drawColor = Color.green;
#endif
                        }
                    }
#if DRAW_STEP_BY_STEP
                    Debug.DrawLine(drawPos1, drawPos2, drawColor, drawTime);
#endif
                }
#if DRAW_STEP_BY_STEP && DRAW_STEP_BY_STEP_DELAY
                var delaySec = .1f;
                await Task.Delay(Mathf.RoundToInt(1000 * delaySec));
#endif
                
                steps++;
                stepsBeforeAwait++;
                if (stepsBeforeAwait >= StepsPerFrameMax)
                {
                    // CLog.LogRed($"[AStar] steps... awaiting ------------");
                    stepsBeforeAwait = 0;
                    await Task.Yield();
                }
            }
            return false;
        }

    }
}