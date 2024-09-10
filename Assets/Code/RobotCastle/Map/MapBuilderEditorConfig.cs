using UnityEngine;

namespace Bomber
{
    [System.Serializable]
    public class MapBuilderEditorConfig
    {
        #if UNITY_EDITOR
        [Header("Map layout")]
        public Vector2 rect1_pos;
        public Vector2 rect1_size;
        [Space(10), Header("Current Cell Rect")]
        public Vector2 rect2_pos;
        public Vector2 rect2_size;
        [Space(10), Header("Top buttons Rect")]
        public Vector2 rect3_pos;
        public Vector2 rect3_size;
        [Space(10), Header("Label and refs Rect")]
        public Vector2 rect4_pos;
        public Vector2 rect4_size;
        [Space(10), Header("Joystick Rect")]
        public Vector2 rect5_pos;
        public Vector2 rect5_size;
        [Space(10), Header("Content Rect")]
        public Vector2 rect6_pos;
        public Vector2 rect6_size;
        [Space(10), Header("Content Prefab Name Rect")]
        public Vector2 rect7_pos;
        public Vector2 rect7_size;
        [Space(10), Header("Scene Management Rect")]
        public Vector2 rect8_pos;
        public Vector2 rect8_size;
        [Space(4), Header("Content Spawn Btn")]
        public EditorButtonConfig btn_content_spawn;
        [Space(4), Header("Content Clear Btn")]
        public EditorButtonConfig btn_content_clear;
        [Space(4), Header("Content Prev Btn")]
        public EditorButtonConfig btn_content_prev;
        [Space(4), Header("Content Next Btn")]
        public EditorButtonConfig btn_content_next;
        [Space(4), Header("Treasure Init Btn")]
        public EditorButtonConfig btn_treasure_init; 

        [Header("Colors background")]
        public Color backColor1;
        public Color backColor2;
        [Header("Colors map cells")]
        public Color mapColorDefault = Color.white;
        public Color mapColorSelected = Color.white;
        public Color mapColorHardWall = Color.white;
        public Color mapColorSoftWall = Color.white;
        public Color mapColorOther = Color.white;
        [Header("Buttons colors")] 
        public Color defaultActionBtnColor = Color.white;
        public Color labelColor = Color.white;
        public Color textsColor = Color.white;
        [Space(10)] 
        public int fontSizeMain;
        public int buttonsDownOffset = 20;
        public int cellInfoColumn2Offset = 50;
        public int cellInfoTopOffset = 50;
        public int cellInfoLinesSpace = 5;
        [Header("joystick")] 
        public int btnSize;
        #endif
    }
}