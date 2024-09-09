using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class GridView : MonoBehaviour, IGridView
    {
        [SerializeField] private int _perRow = 5;
        [SerializeField] private List<GameObject> _cellViewGameObjects;
        private MergeGrid _lastGrid;
        
        public int PerRowCount
        {
            get => _perRow;
            set => _perRow = value;
        }
        
        private ICellView[,] _grid;
        public ICellView[,] Grid => _grid;
        public MergeGrid BuiltGrid => _lastGrid;

        public ICellView GetCell(int x, int y)
        {
            return _grid[x, y];
        }

        public void BuildGrid(MergeGrid gridData)
        {
            var count = _cellViewGameObjects.Count;
            var x = 0;
            var y = 0;
            var xCount = gridData.rows[0].Count;
            var yCount = gridData.RowsCount;
            if (count % _perRow != 0)
            {
                CLog.LogRed("count % _perRow != 0. NOT A RECTANGLE!!");
                return;
            }
            _grid = new ICellView[xCount, yCount];
            for (var i = 0; i < count; i++)
            {
                if (x >= xCount)
                {
                    y++;
                    x = 0;
                }
                var go = _cellViewGameObjects[i].gameObject;
                // CLog.Log($"{x}_{y}    {go.name}");
                var view = go.GetComponent<ICellView>();
                _grid[x, y] = view;
                view.cell = gridData.rows[y].cells[x];
                x++;
            }
        }

        public MergeGrid BuildGridFromView()
        {
            var grid = new MergeGrid();
            var count = _cellViewGameObjects.Count;
            var xCount = _perRow;
            var yCount = count / _perRow;
            grid.rows = new List<CellsRow>(yCount);
            _grid = new ICellView[xCount, yCount];
            var linearIndex = 0;
            for (var y = 0; y < yCount; y++)
            {
                var row = new CellsRow() {
                    cells = new List<Cell>(xCount) };
                for (var x = 0; x < xCount; x++)
                {
                    var newCell = new Cell() {
                        isUnlocked = true,
                        currentItem = ItemData.Null,
                        isOccupied = false,
                        x = x,
                        y = y
                    };
                    row.cells.Add(newCell);
                    _grid[x, y] = _cellViewGameObjects[linearIndex].GetComponent<ICellView>();
                    _grid[x, y].cell = newCell;
                    linearIndex++;
                }
                grid.rows.Add(row);                
            }
            _lastGrid = grid;
            return grid;
        }


        public void GetGrid()
        {
            var count = _cellViewGameObjects.Count;
            var x = 0;
            var y = 0;
            var xCount = _perRow;
            var yCount = count / _perRow;
            if (count % _perRow != 0)
            {
                CLog.LogRed("count % _perRow != 0. NOT A RECTANGLE!!");
                return;
            }
            _grid = new ICellView[xCount, yCount];
            for (var i = 0; i < count; i++)
            {
                if (x >= xCount)
                {
                    y++;
                    x = 0;
                }
                var go = _cellViewGameObjects[i].gameObject;
                var view = go.GetComponent<ICellView>();
                _grid[x, y] = view;
            }
        }
        
        
#if UNITY_EDITOR
        [Space(20)]
        public List<Transform> e_transforms;
        public float e_posZ;

        [ContextMenu("E_GetViews")]
        public void E_GetViews()
        {
            var parent = transform;
            var count = parent.childCount;
            var x = 0;
            var y = 0;
            var xCount = _perRow;
            var yCount = count / _perRow;
            if (count % _perRow != 0)
            {
                CLog.LogRed("count % _perRow != 0. NOT A RECTANGLE!!");
                return;
            }
            _grid = new ICellView[xCount, yCount];
            _cellViewGameObjects = new List<GameObject>(count);
            for (var i = 0; i < count; i++)
            {
                if (x >= xCount)
                {
                    y++;
                    x = 0;
                }
                var go = parent.GetChild(i).gameObject;
                _cellViewGameObjects.Add(go);
                var view = go.GetComponent<ICellView>();
                if (view == null)
                {
                    view = go.AddComponent<MergeCellView>();
                    UnityEditor.EditorUtility.SetDirty(go);
                }
                _grid[x, y] = view;
            }
        }

        [ContextMenu("E_SetFrontPos")]
        public void E_SetFrontPos()
        {
            foreach (var cell in e_transforms)
            {
                var tr = cell.GetChild(2);
                var pos = tr.localPosition;
                pos.z = -e_posZ;
                tr.localPosition = pos;
                tr.localEulerAngles = new Vector3(0f, 0f, 0f);
                UnityEditor.EditorUtility.SetDirty(tr);
            }
        }
        
        [ContextMenu("E_SetBackPos")]
        public void E_SetBackPos()
        {
            var half = e_transforms.Count / 2;
            for (var i = 0; i < e_transforms.Count; i++)
            {
                var cell = e_transforms[i];
                var tr = cell.GetChild(2);
                var pos = tr.localPosition;
                if (i < half)
                    pos.z = -e_posZ;
                else
                    pos.z = 0;
                tr.localPosition = pos;
                tr.localEulerAngles = new Vector3(0f, 180, 0f);
                UnityEditor.EditorUtility.SetDirty(tr);
            }
        }
#endif
    }
}