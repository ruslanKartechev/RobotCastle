﻿using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    public class Map : IMap
    {
    
        public Map(Vector2Int size, float cellSize, Vector3 worldOrigin)
        {
            _size = size;
            Grid = new MapCell[size.x, size.y];
            CellSize = cellSize;
            WorldOrigin = worldOrigin;
        }

        
        public List<IAgent> ActiveAgents { get; } = new(20);
        
        public Vector2Int Size => _size;
        
        public MapCell[,] Grid { get; private set;}

        public float CellSize { get; private set; }
        
        public Vector3 WorldOrigin { get; private set;}

        private Vector2Int _size;
        
        public bool IsOutOfBounce(Vector2Int coord)
        {
            if (coord.x >= Size.x || coord.y >= Size.y || coord.x < 0 || coord.y < 0)
                return true;
            return false;
        }
        
        public bool IsFullyFree(Vector2Int coord)
        {
            if (IsOutOfBounce(coord)) 
                return false;
            if (Grid[coord.x, coord.y].isPlayerWalkable == false || 
                Grid[coord.x, coord.y].isAIWalkable == false)
            {
                return false;
            }
            foreach (var ag in ActiveAgents)
            {
                if(ag.CurrentCell == coord || ag.TargetCell == coord)
                    return false;
            }
            return true;
        }
        
        public Vector2Int GetCellPositionFromWorld(Vector3 worldPosition)
        {
            var p = worldPosition - WorldOrigin;
            var x = Mathf.RoundToInt(p.x);
            var y = Mathf.RoundToInt(p.z);
            return new Vector2Int(x, y);
        }

        public Vector3 GetWorldFromCell(Vector2Int cell)
        {
            return WorldOrigin + new Vector3(cell.x * CellSize, 0f, cell.y * CellSize);
        }

   
        public MapCell GetCellAtPosition(Vector3 worldPosition)
        {
            var p = worldPosition - WorldOrigin;
            var x = Mathf.RoundToInt(p.x);
            var y = Mathf.RoundToInt(p.z);
            return Grid[x, y];
        }
        
        public void GetCellAtPosition(Vector3 worldPosition, out Vector2Int coord, out MapCell cell)
        {
            var p = worldPosition - WorldOrigin;
            var x = Mathf.RoundToInt(p.x);
            var y = Mathf.RoundToInt(p.z);
            coord = new Vector2Int(x, y);
            cell = Grid[x, y];
        }
        
        
        public bool GetIfWalkable(Vector3 worldPosition)
        {
            var p = worldPosition - WorldOrigin;
            var x = Mathf.RoundToInt(p.x);
            if (x >= _size.x || x < 0) return false;
            var y = Mathf.RoundToInt(p.z);
            if (y >= _size.y || y < 0) return false;
            return Grid[x, y].isPlayerWalkable;
        }

        public void SetWalkable(int x, int y, bool walkable)
        {
            Grid[x, y].isPlayerWalkable = walkable; 
            Grid[x, y].isAIWalkable = walkable;
        }
        
        public void SetWalkableAll(int x, int y)
        {
            Grid[x, y].isPlayerWalkable = true;
            Grid[x, y].isAIWalkable = true;
        }
        
        public void FillFromEditor(EditorMap editorMap)
        {
            for (var y = 0; y < _size.y; y++)
            {
                for (var x = 0; x < _size.x; x++)
                {
                    var wc = editorMap.Rows[y].cells[x];
                    var cell = new MapCell()
                    {
                        blockType = wc.blockType,
                        isAIWalkable = wc.isAIWalkable,
                        isPlayerWalkable = wc.isPlayerWalkable,
                        worldPosition = wc.worldPosition
                    };
                    // cell.neighbours = MapUtils.GetNeighbours(_size, new Vector2Int(x, y));
                    Grid[x, y] = cell;
                    
                }                
            }
        }
        
    }
}