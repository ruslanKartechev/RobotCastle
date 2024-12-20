﻿using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class GridView : MonoBehaviour, IGridView
    {
        [SerializeField] private int _gridId;
        [SerializeField] private int _perRow = 7;
        [SerializeField] private List<GameObject> _cellViewGameObjects;
        private MergeGrid _lastGrid;

        public int GridId => _gridId;
        
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
                var cellView = go.GetComponent<ICellView>();
                cellView.GridId = _gridId;
                _grid[x, y] = cellView;
                cellView.cell = gridData.rows[y].cells[x];
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
                    _grid[x, y].GridId = _gridId;
                    linearIndex++;
                }
                grid.rows.Add(row);                
            }
            _lastGrid = grid;
            return grid;
        }

      
        
#if UNITY_EDITOR
        [Space(20)]
        [Header("EDITOR ONLY")]
        public List<Transform> e_transforms;
        public float e_posZ;
        public List<Material> e_materials;
        public string e_mat_holder_id = "cube outer";
        public Vector3 e_spawnPointRotation;

        [ContextMenu("Get Views")]
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

        [ContextMenu("Set Rotation")]
        public void E_SetRotation()
        {
            foreach (var cell in _cellViewGameObjects)
            {
                var spawnPoint = cell.transform.GetChild(0);
                spawnPoint.rotation = Quaternion.Euler(e_spawnPointRotation);
                UnityEditor.EditorUtility.SetDirty(spawnPoint);
            }
        }

        [ContextMenu("Set Spawn Positions")]
        public void E_SetSpawnPositions()
        {
            var rowNum = 1;
            var cellNum = 1;
            foreach (var cell in _cellViewGameObjects)
            {
                CLog.Log($"{cell.name} Cell num {cellNum}. Row {rowNum}");
                var tr = cell.transform.GetChild(0);
                if (tr.name.Contains("item point") == false)
                    continue;
                var pos = tr.localPosition;
                var eulers = new Vector3();
                if (rowNum <= 2)
                {
                    pos.z = 0f;
                    eulers = new Vector3(0f, 180f, 0f);
                    CLog.LogBlue($"{cell.name} Cell num {cellNum}. Row {rowNum}");
                }
                else
                {
                    pos.z = -e_posZ;
                    eulers = new Vector3(0f, 0f, 0f);
                }
                tr.localPosition = pos;
                tr.localEulerAngles = eulers;
                cellNum++;
                if (cellNum > 7)
                {
                    cellNum = 1;
                    rowNum++;
                }
                UnityEditor.EditorUtility.SetDirty(tr);
            }
        }
        
        [ContextMenu("SetSpawnPositionsForEnemies")]
        public void E_SetSpawnPositionsForEnemies()
        {
            var rowNum = 1;
            var cellNum = 1;
            foreach (var cell in _cellViewGameObjects)
            {
                CLog.Log($"{cell.name} Cell num {cellNum}. Row {rowNum}");
                var tr = cell.transform.GetChild(0);
                if (tr.name.Contains("item point") == false)
                    continue;
                tr.localPosition = Vector3.zero;
                tr.localEulerAngles = Vector3.zero;
                cellNum++;
                if (cellNum > 7)
                {
                    cellNum = 1;
                    rowNum++;
                }
                UnityEditor.EditorUtility.SetDirty(tr);
            }
        }

        

        [ContextMenu("Set FrontPos")]
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
        
        [ContextMenu("Set BackPos")]
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
        
        [ContextMenu("Set Material To Selected")]
        public void E_SetMaterialToSelected()
        {
            var matInd = 0;
            foreach (var tr in e_transforms)
            {
                for (var i = 0; i < tr.childCount; i++)
                {
                    var child = tr.GetChild(i).gameObject;
                    if (child.name.Contains(e_mat_holder_id))
                    {
                        SetMat(child);
                        break;
                    }
                }
            }

            void SetMat(GameObject go)
            {
                var rend = go.GetComponent<MeshRenderer>();
                rend.sharedMaterial = e_materials[matInd];
                matInd++;
                if (matInd >= e_materials.Count)
                    matInd = 0;
                UnityEditor.EditorUtility.SetDirty(rend);
            }
        }
        
        [ContextMenu("Set Selected Hidden")]
        public void E_SetSelectedHidden()
        {
            E_SetSelectedState(false);
        }
        
              
        [ContextMenu("Set Selected Shown")]
        public void E_SetSelectedShown()
        {
            E_SetSelectedState(true);
        }
        
              
        public void E_SetSelectedState(bool state)
        {
            foreach (var tr in e_transforms)
            {
                for (var i = 0; i < tr.childCount; i++)
                {
                    var child = tr.GetChild(i).gameObject;
                    if (child.name.Contains(e_mat_holder_id))
                    {
                        child.gameObject.SetActive(state);
                        var coll = tr.GetComponent<Collider>();
                        if (coll != null)
                            coll.enabled = state;
                        UnityEditor.EditorUtility.SetDirty(child.gameObject);
                        break;
                    }
                }
            }
        }
        

        
        [ContextMenu("E_SetMaterialsToAll")]
        public void E_SetMaterialsToAll()
        {
            var matInd = 0;
            foreach (var go in _cellViewGameObjects)
            {
                for (var i = 0; i < go.transform.childCount; i++)
                {
                    var child = go.transform.GetChild(i).gameObject;
                    if (child.name.Contains(e_mat_holder_id))
                    {
                        SetMat(child);
                        break;
                    }
                }
            }

            void SetMat(GameObject go)
            {
                var rend = go.GetComponent<MeshRenderer>();
                rend.sharedMaterial = e_materials[matInd];
                matInd++;
                if (matInd >= e_materials.Count)
                    matInd = 0;
                UnityEditor.EditorUtility.SetDirty(rend);
            }
        }
#endif
    }
}