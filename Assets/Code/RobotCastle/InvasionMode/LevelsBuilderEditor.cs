#if UNITY_EDITOR
using System;
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.InvasionMode
{
    [CustomEditor(typeof(LevelsBuilder))]
    public class LevelsBuilderEditor : Editor
    {
        private const int pickBtnSize = 36;
        private const int cellSize = 40;
        private const int cellSpace = 3;
        private const int fontSize = 16;
        
        private Color backColor1 = Color.yellow * .15f;
        private Color backColor2 = Color.blue * .15f;
        private bool _isBackgroundFirst;
        
        private float rectStartY = 380;
        private float rectY = 0;
        private int totalHeight = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as LevelsBuilder;
            totalHeight = 0;
            if (Event.current.type == EventType.Repaint)
            {
                // hack to get real view width
                var r = GUILayoutUtility.GetLastRect();
                rectStartY = r.position.y + r.height + 20;
            }            

            rectY = rectStartY;
            _isBackgroundFirst = true;
            
            if (!me.showEditor)
                return;
            
            var parentRect = NextRectDown(50);
            totalHeight += 50;
            DrawBackgroundColor(parentRect);
            GUILayout.BeginArea(parentRect);
            EU.Space();
            
            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide3("Build New Files", EU.Crimson))
                me.GenerateFilesAllNewFilesIfDontExist();
            if (EU.BtnMidWide2("Read Again", EU.Gold))
                me.ReadCurrentPreset();
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        
            var doShowMap = ShowFilePickButtons();
            
            if(doShowMap)
            {
               ShowMap();
               ShowEnemyControlButtons();
               ShowCellEditor();
            }
            GUILayout.Space(totalHeight + 40);
        }

        private bool ShowFilePickButtons()
        {
            var me = target as LevelsBuilder;
            var parentRect = NextRectDown(150);
            totalHeight += 150;
            DrawBackgroundColor(parentRect);
            GUILayout.BeginArea(parentRect);
            EU.Space();
            if (me.currentPresetFile == null)
            {
                EU.Label("Choose a preset file", Color.yellow, fontSize, 'c', true);
            }
            else
            {
                var presetName = me.currentPresetFile.name;
                EU.Label($"File: {presetName}", Color.yellow, fontSize, 'c', true);
            }

            GUILayout.BeginHorizontal();
            if (EU.BtnSquare("<<", Color.green, pickBtnSize))
                me.PrevFile();
            if (EU.BtnSquare(">>", Color.green, pickBtnSize))
                me.NextFile();
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            var doShowMap = false;
            if (me.currentPresetFile != null && me.currentPack != null)
            {
                EU.Space();
                GUILayout.BeginHorizontal();
                if (EU.BtnMidWide3("Save Changes", Color.green))
                    me.Save();
                GUILayout.Space(4);
                if (EU.BtnMidWide2("Clear All", Color.red))
                    me.ClearAllEnemies();
                GUILayout.EndHorizontal();
                EU.Label("Don't Forget to SAVE", EU.White, 'l', false);
                doShowMap = true;
            }
            GUILayout.EndArea();
            return doShowMap;
        }

   
        private void ShowMap()
        {
            var me = target as LevelsBuilder;
            var mapH = me.gridSize.y;
            var mapW = me.gridSize.x;
            var parentRect = NextRectDown(280);
            totalHeight += 280;

            DrawBackgroundColor(parentRect);
            GUILayout.BeginArea(parentRect);
            
            GUILayout.Space(5);
            EU.Label("Grid layout", Color.yellow, fontSize);
            GUILayout.Space(5);

            var topRowY = 30;
            var lowLeftCellPos = new Vector2(0, topRowY + (cellSize * (mapH + 1)) + cellSpace * mapH);
            var currentPos = lowLeftCellPos;
            var labelPos = new Vector2(0, topRowY);
            for (var x = 0; x < mapW; x++)
            {
                var rect = new Rect(labelPos, new Vector2(cellSize, cellSize));
                EU.LabelRect(rect, (x+1).ToString(), fontSize - 2, Color.white, 'c', true);
                labelPos.x += cellSize + cellSpace;
            }
            for (var y = 0; y < mapH; y++)
            {
                currentPos.y -= cellSize + cellSpace;
                currentPos.x = lowLeftCellPos.x;
                for (var x = 0; x < mapW; x++)
                {
                    var color = Color.gray;
                    var currentCoord = new Vector2Int(x, y);
                    // var id = "";
                    var texture = (Texture2D)null;
                    if (me.HasEnemyAt(currentCoord, out var enemy))
                    {
                        color = Color.white;
                        // id = enemy.enemy.id[0].ToString();
                        var asset = Resources.Load<Sprite>($"sprites/hero_icon_{enemy.enemy.id}");
                        if (asset != null)
                        {
                            texture = AssetPreview.GetAssetPreview(asset);
                        }
                    }
                    if (currentCoord == me.currentCellCoord)
                        color = Color.yellow;
                    var click = ShowCellButton(currentPos, color, texture);
                    if (click)
                    {
                        me.SetCell(currentCoord);
                    }
                    currentPos.x += cellSize + cellSpace;
                }
            }
            GUILayout.EndArea();
        }

        private void ShowEnemyControlButtons()
        {
            var me = target as LevelsBuilder;
            const int btnSize = 40;
            var parentRect = NextRectDown(100);
            totalHeight += 100;
            DrawBackgroundColor(parentRect);
            GUILayout.BeginArea(parentRect);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (EU.BtnSquare("<<", EU.Moccasin, btnSize))
                me.PrevEnemyPreset();
            GUILayout.Space(5);
            if (EU.BtnSquare(">>", EU.Moccasin, btnSize))
                me.NextEnemyPreset();
            
            var preset = me.GetCurrentPreset();
            var id = "none";
            if (preset != null)
                id = preset.enemy.id;
            EU.Label(id, Color.white, fontSize, 'l');
            
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("Set", Color.green))
                me.SetEnemyToChosenCell();
            GUILayout.Space(5);
            if (EU.BtnMid2("Clear", new Color(1, .3f, .3f)))
                me.Clear();                

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        
        private void ShowCellEditor()
        {
            var me = target as LevelsBuilder;
            var parentRect = NextRectDown(250);
            totalHeight += 250;

            DrawBackgroundColor(parentRect);
            GUILayout.BeginArea(parentRect);
            var configMain = serializedObject.FindProperty("currentEnemy");
            if(configMain != null)
            {
                EditorGUILayout.PropertyField(configMain, new GUIContent("Current Enemy"));
                GUILayout.BeginHorizontal();
                Texture2D texture = null;
                var hasTexture = false;
                if (me.currentEnemy != null)
                {
                    var asset = Resources.Load<Sprite>($"sprites/hero_icon_{me.currentEnemy.enemy.id}");
                    texture = AssetPreview.GetAssetPreview(asset);
                    hasTexture = texture != null;
                }
                if(hasTexture)
                {
                    GUILayout.Label(texture);
                    var mergeLevel = me.currentEnemy.enemy.level;
                    GUILayout.BeginVertical();
                    GUILayout.Space(10);
                    EU.Label($"Merge Level: {mergeLevel}", 'l', true);
                    EU.Label($"ID: {me.currentEnemy.enemy.id}", 'l', true);
                
                    GUILayout.Space(10);
                    
                    GUILayout.BeginHorizontal();
                    if (EU.BtnSquare("-", Color.green, 30, fontSize-1))
                    {
                        mergeLevel--;
                        if (mergeLevel >= 0)
                            me.currentEnemy.enemy.level = mergeLevel;
                    }
                    if (EU.BtnSquare("+", Color.green, 30, fontSize-1))
                    {
                        mergeLevel++;
                        if(mergeLevel < 7)
                            me.currentEnemy.enemy.level = mergeLevel;
                    }
                    GUILayout.EndHorizontal();
                
                    GUILayout.EndVertical();
                }
         
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label((Texture)null);
            }

            GUILayout.EndArea();
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawBackgroundColor(Rect rect)
        {
            var color = _isBackgroundFirst ? backColor1 : backColor2;
            _isBackgroundFirst = !_isBackgroundFirst;
            var backgroundRect = new Rect(rect);
            backgroundRect.position = new Vector2(0, backgroundRect.position.y);
            backgroundRect.width = Screen.width;
            EditorGUI.DrawRect(backgroundRect,  color);
        }
        
        private bool ShowCellButton(Vector2 pos, Color color, string id)
        {
            var rect = new Rect(pos, new Vector2(cellSize, cellSize));
            GUILayout.BeginArea(rect);
            var res = EU.Button(new GUIContent(id), cellSize,cellSize, color);
            GUILayout.EndArea();
            return res;
        }
        
        private bool ShowCellButton(Vector2 pos, Color color, Texture texture)
        {
            var rect = new Rect(pos, new Vector2(cellSize, cellSize));
            GUILayout.BeginArea(rect);
            var content = (GUIContent)null;
            if (texture == null)
                content = new GUIContent("");
            else
                content = new GUIContent(texture);
            var res = EU.Button(content, cellSize,cellSize, color);
            GUILayout.EndArea();
            return res;
        }
        
        private Rect NextRectDown(float height)
        {
            var rect = new Rect(15, rectY, 300, height);
            rectY += height;
            return rect;
        }
    }
}
#endif