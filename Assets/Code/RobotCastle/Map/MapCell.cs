using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    public struct MapCell
    {
        public bool isPlayerWalkable;
        public bool isAIWalkable;
        public EMapBlockType blockType;
        public List<Vector2Int> neighbours;
        public Vector3 worldPosition;
    }
}