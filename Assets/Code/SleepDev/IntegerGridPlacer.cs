using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SleepDev
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(IntegerGridPlacer))]
    public class IntegerGridPlacerEditor : Editor
    {
        private const int font_size = 16;
        private const int pos_label_width = 130;
        
        
        public override void OnInspectorGUI()
        {
            var me = target as IntegerGridPlacer;
            base.OnInspectorGUI(); 
           
            GUILayout.Space(10);
            // EU.Label($"X Axis", EU.Aqua);
            var mainRect = new Rect(new Vector2(20, 100), new Vector2(500, 200));
            const int b_rect_side = 37;
            var b_rect_1 =  new Rect(new Vector2(b_rect_side, 0), new Vector2(b_rect_side, b_rect_side));
            var b_rect_2 =  new Rect(new Vector2(2 * b_rect_side, b_rect_side), new Vector2(b_rect_side, b_rect_side));
            var b_rect_3 =  new Rect(new Vector2(b_rect_side, 2 * b_rect_side), new Vector2(b_rect_side, b_rect_side));
            var b_rect_4 =  new Rect(new Vector2(0, b_rect_side), new Vector2(b_rect_side, b_rect_side));
            
            var rect_info =  new Rect(new Vector2(3 * b_rect_side, 0), new Vector2(300, 200));
            var rect_buttons =  new Rect(new Vector2(20, 220), new Vector2(300, 200));

            GUILayout.BeginArea(mainRect);
            {
                GUILayout.BeginArea(b_rect_1);
                if (EU.BtnSquare("U", EU.Gold,  b_rect_side-1, font_size))
                {
                    me.NextZ();
                }
                GUILayout.EndArea(); 
            }
            {
                GUILayout.BeginArea(b_rect_2);
                if (EU.BtnSquare("R", EU.Gold, b_rect_side-1,font_size))
                {
                    me.NextX();
                }
                GUILayout.EndArea();
            }
            {
                GUILayout.BeginArea(b_rect_3);
                if (EU.BtnSquare("D", EU.Gold,b_rect_side-1, font_size))
                {
                    me.PrevZ();
                }
                GUILayout.EndArea();
            }
            {
                GUILayout.BeginArea(b_rect_4);
                if (EU.BtnSquare("L", EU.Gold, b_rect_side-1, font_size))
                {
                    me.PrevX();
                }
                GUILayout.EndArea();
            }
            {
                GUILayout.BeginArea(rect_info);
                GUILayout.BeginVertical();
                EU.Label($"X-axis:  {me.x}", EU.Aqua, font_size, pos_label_width);
                EU.Label($"Z-axis:  {me.z}", EU.Aqua, font_size, pos_label_width);
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(rect_buttons);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("Round", EU.Gold))
            {
                me.Round();
            }
            GUILayout.Space(15);
            if (EU.BtnMid2("Place", EU.Gold))
            {
                me.SetPos();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.Space(300);
        }
    }
    #endif
    
    public class IntegerGridPlacer : MonoBehaviour
    {
        #if UNITY_EDITOR
        public int x;
        public int z;
        public bool autoSet;

        private void OnValidate()
        {
            x = Mathf.RoundToInt(transform.position.x);
            z = Mathf.RoundToInt(transform.position.z);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void Round()
        {
            x = Mathf.RoundToInt(transform.position.x);
            z = Mathf.RoundToInt(transform.position.z);
            SetPos();
        }

        public void NextX()
        {
            x++;
            if(autoSet)
                SetPos();
        }

        public void PrevX()
        {
            x--;
            if(autoSet)
                SetPos();
        }

        public void NextZ()
        {
            z++;
            if(autoSet)
                SetPos();
        }

        public void PrevZ()
        {
            z--;
            if(autoSet)
                SetPos();
        }

        public void SetPos()
        {
            transform.position = new Vector3(x, transform.position.y, z);
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}