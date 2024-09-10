#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace Bomber
{
    public class MapEditorWindow : EditorWindow
    {
        private const int cellSize = 18;
        private const int cellSpace = 2;
        
        private const int fontError = 20;
        private const int fontLabel = 30;
        private const int fontFields = 14;
        private const int sideOffset = 10;
        
        
        private MapBuilder _me;
        private MapBuilderEditorConfig _config;
        private MapBuilderEditorConfigSO _configSO;
        private Vector2 _mapRectSize;
        private float _windowWidth = 100;
        private float _lastRowOffset;
        private Vector2 _scrollPos = Vector2.zero;
        private bool _backColor1;

        
        private void DrawBackgroundColor(Rect rect)
        {
            var color = _backColor1 ? _config.backColor1 : _config.backColor2;
            _backColor1 = !_backColor1;
            EditorGUI.DrawRect(rect,  color);
        }

        [MenuItem("SleepDev/Map Editor")]
        public static void ShowWindow()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<MapEditorWindow>();
            wnd.titleContent = new GUIContent("Map Editor");
            
        }
        
        public void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            if (_me == null)
            {
                _me = FindObjectOfType<MapBuilder>();
                if (_me == null)
                {
                    CLog.LogRed($"MapBuilder not found in the scene");
                }
            }

            if (_configSO == null && _config == null)
            {
                _configSO = Resources.Load<MapBuilderEditorConfigSO>("Map Builder EditorConfig");
                if (_configSO != null)
                    _config = _configSO.config;
            }
        }

        public void OnGUI()
        {
            _backColor1 = true;
            _windowWidth = position.width;
            if (_me == null)
            {
                EU.Label("Error: Map builder not found in the scene...",  Color.red,fontError, 'l');
                _me = FindObjectOfType<MapBuilder>();
                return;
            }

            _lastRowOffset = 0f;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, true);
 
            ShowLabelAndSettings();
            if (_config == null)
                return;
            GUILayout.Box(GUIContent.none, GUILayout.Width(position.width), GUILayout.Height(1000));
            ShowTopButtons(); 
            ShowMapLayout();
            ShowCurrentInfo();
            ShowContentControls();
            ShowSceneManagementButtons();
            EditorGUILayout.EndScrollView();
        }

        private void ShowLabelAndSettings()
        {
            if (_config == null)
            {
                var rect = new Rect(new Vector2(0, 0), new Vector2(_windowWidth, 100));
                _lastRowOffset += 100;
                DrawBackgroundColor(rect);
                GUILayout.BeginArea(rect);
                EU.Label("Map Editor", EU.White, fontLabel);
                _configSO = (MapBuilderEditorConfigSO)EU.ObjectField("Settings", fontsize: fontFields, EU.White, 60, _configSO, typeof(MapBuilderEditorConfigSO));
                if (_configSO != null)
                    _config = _configSO.config;
                GUILayout.EndArea();
            }
            else
            {
                var rect = new Rect(_config.rect4_pos, _config.rect4_size);
                rect.width = _windowWidth; 
                _lastRowOffset += rect.height;
                GUILayout.BeginArea(rect);
                EU.Label("Map Editor", _config.labelColor, fontLabel);
                GUILayout.Space(6);
                _configSO = (MapBuilderEditorConfigSO)EU.ObjectField("Settings", fontsize: fontFields, _config.textsColor, 60, _configSO, typeof(MapBuilderEditorConfigSO));
                if (_configSO != null)
                    _config = _configSO.config;
                GUILayout.EndArea();
            }
        }

        private void ShowTopButtons()
        {
            _config.rect3_size.x = _windowWidth;
            var rect = new Rect(new Vector2(0f, _lastRowOffset), _config.rect3_size);
            _lastRowOffset += rect.height;
            DrawBackgroundColor(rect);
            GUILayout.BeginArea(rect);
            const int verticalOffset = 5;
            GUILayout.Space(verticalOffset);
            GUILayout.BeginHorizontal();
            GUILayout.Space(sideOffset);
            if(EU.BtnMidWide2("Scan build map", EU.Chartreuse))
                _me.ScanToBuild();
            if(EU.BtnMidWide("Draw", EU.Chartreuse))
                _me.DrawWithRays();

            GUILayout.Space(5);
            if(EU.BtnMidWide2("Build empty map", EU.Chartreuse))
                _me.BuildEmptyEditorMap();
            GUILayout.Space(5);
            if(EU.BtnMidWide2("Check Walkable", EU.Chartreuse))
                _me.ScanSetWalkable();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        
        private void ShowMapLayout()
        {
            var mapSize = _me.Size;
            var mapH = mapSize.y;
            var areaSize = new Vector2(_windowWidth, 100);
            areaSize.y = (_config.rect1_size.y + cellSize) * mapH + cellSpace * (mapH-1) + _config.buttonsDownOffset;
            var rect = new Rect(new Vector2(0,_lastRowOffset), areaSize);
            _mapRectSize = areaSize;
            _lastRowOffset += rect.height;
            DrawBackgroundColor(rect);
            GUILayout.BeginArea(rect);
            var startPos = new Vector2(5, cellSize * mapH + (mapH - 1) * cellSpace + _config.buttonsDownOffset);
            var currentPos = startPos;
            var btnRectSize = new Vector2(cellSize, cellSize);
            for (var y = 0; y < mapSize.y; y++)
            {
                currentPos.y -= cellSize + cellSpace;
                currentPos.x = startPos.x;
                for (var x = 0; x < mapSize.x; x++)
                {
                    currentPos.x += cellSize + cellSpace;
                    var color = _config.mapColorDefault;
                    var btnPos = new Vector2Int(x, y); 
                    if (btnPos == _me.CurrentPos)
                        color = _config.mapColorSelected;
                    else
                    {
                        switch (_me.GetCellAt(x, y).blockType)
                        {
                            case EMapBlockType.SoftWall:
                                color = _config.mapColorSoftWall;
                                break;
                            case EMapBlockType.HardWall:
                                color = _config.mapColorHardWall;
                                break;
                            case EMapBlockType.Other:
                                color = _config.mapColorOther;
                                break;
                        }
                    }
                    if (ShowCellButton(currentPos, btnRectSize, color))
                    {
                        _me.CurrentPos = btnPos;
                    }
                } 
            }
            GUILayout.EndArea();
        }
        
        private void ShowControlsJoystick()
        {
            // var mapSize = _me.Size;
            // var areaPos =_config.rect5_pos ;
            // areaPos.x += (_config.rect1_size.x + cellSize) * mapSize.x + cellSpace * (mapSize.x-1);
            var rect = new Rect(_config.rect5_pos, _config.rect5_size);
            GUILayout.BeginArea(rect);
            var btnSize = _config.btnSize;
            var btnRectSize = new Vector2(_config.btnSize, _config.btnSize);
            var color = _config.defaultActionBtnColor;
            var center = new Vector2(btnSize, btnSize);
            var rectTop = new Rect(center + new Vector2(0, -btnSize), btnRectSize);
            var rectRight = new Rect(center + new Vector2(btnSize, 0f), btnRectSize);
            var rectBot = new Rect(center + new Vector2(0f, btnSize), btnRectSize);
            var rectLeft = new Rect(center + new Vector2(-btnSize, 0), btnRectSize);
            
            GUILayout.BeginArea(rectTop);
            if (EU.BtnSquare("T", color, btnSize))
            {
                _me.MoveCurrentCell(new Vector2Int(0,1));
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(rectRight);
            if (EU.BtnSquare("R", color, btnSize))
            {
                _me.MoveCurrentCell(new Vector2Int(1,0));
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(rectBot);
            if (EU.BtnSquare("D", color, btnSize))
            {
                _me.MoveCurrentCell(new Vector2Int(0,-1));
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(rectLeft);
            if (EU.BtnSquare("L", color, btnSize))
            {
                _me.MoveCurrentCell(new Vector2Int(-1,0));
            }
            GUILayout.EndArea();
            GUILayout.EndArea();
        }


        private void ShowCurrentInfo()
        {
            _config.rect2_size.x = _windowWidth;
            // var areaPos = _config.rect2_pos;
            // areaPos.y += _mapRectSize.y;
            var rect = new Rect(new Vector2(0,_lastRowOffset), _config.rect2_size);
            rect.width = _windowWidth;
            _lastRowOffset += rect.size.y;
            DrawBackgroundColor(rect);
            var sideOffset = _config.cellInfoColumn2Offset;
            var rect1 = new Rect(rect);
            rect1.x = MapEditorWindow.sideOffset;
            rect1.y += _config.cellInfoTopOffset;

            var rect2 = new Rect(rect);
            rect2.x += sideOffset;
            rect2.y += _config.cellInfoTopOffset;
            
            GUILayout.BeginArea(rect1);
            EU.Label("Cell:", _config.textsColor, _config.fontSizeMain, 'l');
            GUILayout.Space(_config.cellInfoLinesSpace);
            EU.Label("Content:", _config.textsColor, _config.fontSizeMain, 'l');
            GUILayout.EndArea();
            
            GUILayout.BeginArea(rect2);
            EU.Label($"(x,y) ({_me.CurrentPos.x},{_me.CurrentPos.y})", _config.textsColor, _config.fontSizeMain, 'l');
            GUILayout.Space(_config.cellInfoLinesSpace);
            EU.Label($"{GetCurrentContent()}", _config.textsColor, _config.fontSizeMain, 'l');
            
            ShowControlsJoystick();

            GUILayout.EndArea();
        }

        private void ShowContentControls()
        {
            var rect = new Rect(_config.rect6_pos + new Vector2(0, _lastRowOffset), _config.rect6_size);
            _lastRowOffset += rect.size.y;
            rect.width = _windowWidth;
            DrawBackgroundColor(rect); 
            GUILayout.BeginArea(rect);
            _config.btn_content_spawn.Draw(_me.SpawnContentAtCurrentCell);
            _config.btn_content_clear.Draw(_me.ClearContentAtCurrentCell);
            var labelRect = new Rect(_config.rect7_pos, _config.rect7_size);
            EU.LabelRect(labelRect, $"Prefab: {_me.CurrentPrefabName()}", _config.fontSizeMain, EU.White, 'l', true);
            _config.btn_content_prev.Draw(_me.PrevPrefab);
            _config.btn_content_next.Draw(_me.NextPrefab);
            GUILayout.EndArea();
        }

        private void ShowSceneManagementButtons()
        {
            var rect = new Rect(_config.rect8_pos + new Vector2(0, _lastRowOffset), _config.rect8_size);
            _lastRowOffset += rect.size.y;
            rect.width = _windowWidth;
            DrawBackgroundColor(rect); 
            GUILayout.BeginArea(rect);
            _config.btn_treasure_init.Draw(_me.TryInitTreasure);
            GUILayout.EndArea();
            
        }
    
        private string GetCurrentContent()
        {
            var cell = _me.GetCellAt(_me.CurrentPos.x, _me.CurrentPos.y);
            if (cell.IsEmpty)
                return "Empty";
            return cell.content.gameObject.name;
        }

        private bool ShowCellButton(Vector2 pos, Vector2 btnRectSize, Color color)
        {
            var oldColor = GUI.color;
            GUI.color = color;
            var rect = new Rect(pos, btnRectSize);
            GUILayout.BeginArea(rect);
            // GUILayout.Width(_cellSize), GUILayout.Height(_cellSize)
            var res = GUILayout.Button("");
            GUILayout.EndArea();
            GUI.color = oldColor;
            return res;
        }
    }
}
#endif