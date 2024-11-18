using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.InvasionMode;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Saving
{
    [CreateAssetMenu(menuName = "SO/EditorHelper", fileName = "EditorHelper", order = -110)]
    public class EditorHelper : ScriptableObject
    {
        public Vector2Int range;
        public List<Vector2Int> rangeCells;
        public Vector2Int gridSize;
        public int lockedBorders;
        [SerializeField] private SavesDataBase _savesDataBase;

#if UNITY_EDITOR
        [Space(20)]
        public LevelData data;
        [Space(20)]

        [Space(20)]
        public string e_level_str;

        [ContextMenu("Fill")]
        public void E_Fill()
        {
            var count = 20;
            data.levels = new List<RoundData>(count);
            for (var lvl = 1; lvl <= count; lvl++)
            {
                var round = new RoundData();
                round.reward = 10;
                round.enemyPreset = $"{e_level_str}_{lvl}";
                switch (lvl)
                {
                    case 4 or 9 or 14 or 19:
                        round.roundType = RoundType.Smelting;
                        break;
                    case 5 or 10 or 15:
                        round.roundType = RoundType.EliteEnemy;
                        break;
                    case 20:
                        round.roundType = RoundType.Boss;
                        break;
                    default:
                        round.roundType = RoundType.Default;
                        break;
                }
                data.levels.Add(round);
                
            }
        }
        
        [ContextMenu("GenerateSquareRange")]
        public void GenerateSquareRange()
        {
            var s = new AttackRangeRectangle(range.x, range.y);
            rangeCells = s.GetCellsMask();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("SetupMergeGrid")]
        public void SetupMergeGrid()
        {
            var grid = new MergeGrid();
            grid.rows = new List<CellsRow>(gridSize.y);
            for (var y = 0; y < gridSize.y; y++)
            {
                grid.rows.Add(new CellsRow() {
                    cells = new List<Cell>(gridSize.x)
                });
                for (var x = 0; x < gridSize.x; x++)
                {
                    grid.rows[y].cells.Add(new Cell(x,y));
                }
            }

            for (var x = 0; x < lockedBorders; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    grid.rows[y].cells[x].isUnlocked = false;
                    grid.rows[y].cells[x].isOccupied = false;

                    grid.rows[y].cells[gridSize.x - 1 - x].isUnlocked = false;
                    grid.rows[y].cells[gridSize.x - 1 - x].isOccupied = false;
                }
            }
            UnityEditor.EditorUtility.SetDirty(_savesDataBase);
        }
#endif
        
    }
}