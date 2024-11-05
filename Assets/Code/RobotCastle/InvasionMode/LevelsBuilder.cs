using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RobotCastle.Battling;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.InvasionMode
{
    public class LevelsBuilder : MonoBehaviour
    {
        public Vector2Int gridSize;
        public List<EnemyPreset> enemyOptions;
        public bool showEditor;
        public List<TextAsset> presetFiles;
        [HideInInspector] public int presetIndex;
        [HideInInspector] public int enemyIndex;
        [HideInInspector] public TextAsset currentPresetFile;
        [HideInInspector] public Vector2Int currentCellCoord;
        [Space(10)]
        [SerializeField] private ProgressionDataBaseContainer _dbContainer;
        [SerializeField] private EnemiesEditorGrid _enemiesGrid;
        [Header("For editing"), Space(10)]
        public EnemyPackPreset currentPack;
        public EnemyPreset currentEnemy;
        
        public EnemiesEditorGrid enemiesGrid => _enemiesGrid;

        [System.Serializable]
        public class EnemiesEditorGrid
        {
            public EnemiesEditorGrid(Vector2Int size)
            {
                this.size = size;
                rows = new List<GridRow>(size.y);
                for (var y = 0; y < size.y; y++)
                {
                    var columns = new List<EnemyPreset>(size.x);
                    for (var x = 0; x < size.x; x++)
                    {
                        columns.Add(new EnemyPreset()
                        {
                            gridPos = new Vector2Int(x, y)
                        });
                    }
                    rows.Add(new GridRow(){columns = columns});
                }
            }

            public Vector2Int size;
            public List<GridRow> rows;

            public EnemyPreset this[int x, int y]
            {
                get => rows[y].columns[x];
                set => rows[y].columns[x] = value;
            }
        }

        [System.Serializable]
        public class GridRow
        {
            public List<EnemyPreset> columns;
        }

        #region SCRIPTS EDITOR HELPERs
        #if UNITY_EDITOR

        [ContextMenu("E_RewriteAll")]
        public void E_RewriteAll()
        {
            var pathParent = GetFoldersPath();
            for (var i = 0; i < presetFiles.Count; i++)
            {
                var srcFile = $"{pathParent}/{presetFiles[i].name}.json";
                var content = System.IO.File.ReadAllText(srcFile);

                var preset = JsonConvert.DeserializeObject<EnemyPackPreset>(content);
                if(preset == null)
                {
                    CLog.LogRed("Preset is null");
                }
                else
                {
                    // CLog.Log($"Ok: {srcFile}");
                    content = JsonConvert.SerializeObject(preset, Formatting.Indented);
                    File.WriteAllText(srcFile, content);
                }
            }
        }

        #endif
        #endregion
        
        public string GetFoldersPath()
        {
            var path = Application.streamingAssetsPath.Replace("StreamingAssets", "Resources");
            path += $"/enemy_presets";
            return path;
        }
        
        public void GenerateFilesAllNewFilesIfDontExist()
        {
            foreach (var chapter in _dbContainer.database.chapters)
            {
                var levels = chapter.levelData.levels;
                var namePreset = chapter.viewName;
                namePreset = namePreset.Replace(" ", "_");
                namePreset = namePreset.ToLower();
                var pathParent = GetFoldersPath();
                for (var i = 0; i < levels.Count; i++)
                {
                    var round = levels[i];
                    var fileName = $"{namePreset}_{i+1}";
                    var path = $"{pathParent}/{fileName}.json";
                    if (File.Exists(path))
                    {
                        CLog.Log($"Already exists: {path}");
                        continue;
                    }
                    CLog.LogWhite($"Creating new: {path}");
                    levels[i].enemyPreset = fileName;
                    var preset = new EnemyPackPreset()
                    {
                        enemies = new List<EnemyPreset>(){new EnemyPreset()}
                    };
                    var content = JsonConvert.SerializeObject(preset, Formatting.Indented);
                    File.WriteAllText(path, content);
                }
                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
            }
        }

        public void NextFile()
        {
            var newIndex = presetIndex + 1;
            if (newIndex < presetFiles.Count && newIndex >= 0)
            {
                presetIndex = newIndex;
                currentPresetFile = presetFiles[newIndex];
                ReadCurrentPreset();
            }
            currentEnemy = null;
        }

        public void PrevFile()
        {
            var newIndex = presetIndex - 1;
            if (newIndex < presetFiles.Count && newIndex >= 0)
            {
                presetIndex = newIndex;
                currentPresetFile = presetFiles[newIndex];
                ReadCurrentPreset();
            }
            currentEnemy = null;
        }

        public void SetCell(Vector2Int cell)
        {
            if (cell.x >= 0 && cell.x < gridSize.x)
            {
                if (cell.y >= 0 && cell.y < gridSize.y)
                {
                    currentCellCoord = cell;
                    this.currentEnemy = _enemiesGrid[cell.x, cell.y];
                }
            }
        }

        public void ClearAllEnemies()
        {
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    enemiesGrid[x, y].enemy.id = "";
                }
            }
        }

        public void RemoveCurrentEnemy()
        {
            if (currentEnemy != null && currentPack != null && currentPack.enemies.Contains(currentEnemy))
            {
                currentEnemy.enemy.id = null;
            }
        }

        public void AddEnemyToCell()
        {
            var preset = new EnemyPreset();
            preset.enemy = new CoreItemData(0, "none", "unit");
            preset.gridPos = currentCellCoord;
            currentPack.enemies.Add(preset);
            currentEnemy = preset;
        }

        public void Save()
        {
            var pathParent = GetFoldersPath();
            var path = $"{pathParent}/{currentPresetFile.name}.json";
            CLog.LogWhite($"Saving file: {path}");
            currentPack.enemies = new List<EnemyPreset>();
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var en = enemiesGrid[x, y];
                    if (en != null && !string.IsNullOrEmpty(en.enemy.id))
                    {
                        en.gridPos = new Vector2Int(x, y);
                        currentPack.enemies.Add(en);
                    }
                }                    
            }
            CLog.Log($"Saving with enemies: {currentPack.enemies.Count}");
            var content = JsonConvert.SerializeObject(currentPack, Formatting.Indented);
            File.WriteAllText(path, content);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public bool HasEnemyAt(Vector2Int coord, out EnemyPreset preset)
        {
            var x= coord.x;
            preset = null;
            if (x < 0 && x >= gridSize.x)
                return false;
            var y= coord.y;
            if (y < 0 && y >= gridSize.y)
                return false;
            preset = enemiesGrid[x, y];
            return preset != null && !string.IsNullOrEmpty(preset.enemy.id);
        }

        public void NextEnemyPreset()
        {
            enemyIndex++;
            enemyIndex = Mathf.Clamp(enemyIndex, 0, enemyOptions.Count - 1);
        }

        public void PrevEnemyPreset()
        {
            enemyIndex--;
            enemyIndex = Mathf.Clamp(enemyIndex, 0, enemyOptions.Count - 1);
        }

        public EnemyPreset GetCurrentPreset()
        {
            if(enemyIndex >= enemyOptions.Count)
                return null;
            return enemyOptions[enemyIndex];
        }

        public void SetEnemyToChosenCell()
        {
            var chosen = GetCurrentPreset();
            if (chosen == null)
                return;
            var temp = new EnemyPreset(chosen);
            temp.gridPos = currentCellCoord;
            _enemiesGrid[currentCellCoord.x, currentCellCoord.y] = temp;
            currentEnemy = temp;
        }

        public void Clear()
        {
            _enemiesGrid[currentCellCoord.x, currentCellCoord.y].enemy 
                = new CoreItemData(0, null, "unit");
        }
        
        private void Dirty()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        public void ReadCurrentPreset()
        {
            if (currentPresetFile == null)
            {
                CLog.LogError("currentPresetFile is null");
                return;
            }
            var content = JsonConvert.DeserializeObject<EnemyPackPreset>(currentPresetFile.text);
            if (content == null)
            {
                CLog.LogError($"Could not deserialize: {currentPresetFile.name}");
                currentPack = new EnemyPackPreset();
            }
            else
            {
                currentPack = content;
                CLog.Log($"Read file: {currentPresetFile.name}. Units count: {content.enemies.Count}");
            }
            _enemiesGrid = new EnemiesEditorGrid(gridSize);
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    enemiesGrid[x, y] = new EnemyPreset() {gridPos = new Vector2Int(x, y)};
                }                    
            }
            if(currentPack.enemies == null)
            {
                currentPack.enemies = new List<EnemyPreset>();
            }
            foreach (var en in currentPack.enemies)
                enemiesGrid[en.gridPos.x, en.gridPos.y] = en;
        }

        private void OnValidate()
        {
            if (enemiesGrid == null || enemiesGrid.size != gridSize) 
            {
                _enemiesGrid = new EnemiesEditorGrid(gridSize);
                for (var x = 0; x < gridSize.x; x++)
                {
                    for (var y = 0; y < gridSize.y; y++)
                    {
                        enemiesGrid[x, y] = new EnemyPreset();
                    }                    
                }
            }
            
            if (presetFiles.Count == 0)
            {
                currentEnemy = null;
                currentPresetFile = null;
                currentPack = null;
            }
            else if (currentPack == null)
            {
                currentEnemy = null;
                currentPresetFile = null;
            }
        }
    }
}