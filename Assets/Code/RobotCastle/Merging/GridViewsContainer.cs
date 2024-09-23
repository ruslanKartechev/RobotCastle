using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Merging
{
    public class GridViewsContainer
    {
        private Dictionary<int, IGridView> _viewsMap;

        public GridViewsContainer()
        {
            _viewsMap = new Dictionary<int, IGridView>(4);
        }

        public void AddGridView(IGridView gridView)
        {
            if (_viewsMap.ContainsKey(gridView.GridId))
            {
                CLog.LogRed($"Grid View with id: {gridView.GridId} already added!");
                return;
            }
            _viewsMap.Add(gridView.GridId, gridView);
        }

        public void RemoveGridView(int id)
        {
            _viewsMap.Remove(id);
        }

        public IGridView GetGridView(int id)
        {
            return _viewsMap[id];
        }
    }
}