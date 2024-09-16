using System.Collections.Generic;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IAttackRange
    {
        List<Vector2Int> GetCellsMask();
        List<Vector2Int> ApplyMask(Vector2Int center);
        
        
        /// <summary>
        /// Max Inclusive. Min Exclusive
        /// </summary>
        List<Vector2Int> ApplyMask(Vector2Int center, int maxX, int maxY);
        
        /// <summary>
        /// Max Inclusive. Min Exclusive
        /// </summary>
        List<Vector2Int> ApplyMask(Vector2Int center, int minX, int minY, int maxX, int maxY);
        
        List<ICellView> ApplyMask(Vector2Int center, IGridView gridView);
        
        /// <summary>
        /// Max Inclusive. Min Exclusive
        /// </summary>
        List<ICellView> ApplyMask(Vector2Int center, IGridView gridView, int minX, int minY);
        
        
    }
}