using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    [System.Serializable]
    public class EditorMap
    {
        [SerializeField] private Vector2Int _size;
        [SerializeField] private List<EditorMapRow> _rows;

        public List<EditorMapRow> Rows
        {
            get => _rows;
            set => _rows = value;
        }
        
        public Vector2Int Size
        {
            get => _size;
            set => _size = value;
        }

        public EditorMap()
        { }

        public EditorMap(Vector2Int size)
        {
            this._size = size;
        }

        public EditorMapCell GetCell(Vector2Int pos)
        {
            return _rows[pos.y].cells[pos.x];
        }
    }

    [System.Serializable]
    public class EditorMapRow
    {
        public List<EditorMapCell> cells;
    }

    
    [System.Serializable]
    public class EditorMapCell
    {
        public Vector3 worldPosition;
        public MapCellContent content;
        public bool isPlayerWalkable;
        public bool isAIWalkable;
        public EMapBlockType blockType;

        public void SetWalkableIfFree()
        {
            if (IsEmpty)
                isPlayerWalkable = isAIWalkable = true;
            else
                isPlayerWalkable = isAIWalkable = false;
        }

        public void SetNonWalkable()
        {
            isPlayerWalkable = isAIWalkable = false;
        }
        
        public bool IsEmpty
        {
            get => content == null;
        }
    }


    public interface IMapManager
    {
        void InitRuntime();
        IMap Map { get; }
    }
    
    //
    // public class MapManager : IMapManager
    // {
    //     private IMap _map;
    //     public IMap Map => _map;
    //
    //     public MapManager(IMap map)
    //     {
    //         _map = map;
    //     }
    //     
    // }
}