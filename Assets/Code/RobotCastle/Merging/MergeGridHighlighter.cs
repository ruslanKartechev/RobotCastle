using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeGridHighlighter : MonoBehaviour
    {
        private IGridView _gridView;
        private List<ICellView> _currentCells;

        public void Init(IGridView gridView)
        {
            _gridView = gridView;
        }
    }
}