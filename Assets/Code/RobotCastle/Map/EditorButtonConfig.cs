#if UNITY_EDITOR
using System;
using SleepDev;
using UnityEngine;

namespace Bomber
{
    [System.Serializable]
    public class EditorButtonConfig
    {
        public Vector2 btn_size;
        public string label;
        public Color btnColor;
        public Vector2 area_size;
        public Vector2 area_pos;

        public void Draw(Action onClick)
        {
            var rect = new Rect(area_pos, area_size);
            GUILayout.BeginArea(rect);
            if (EU.Btn(label, btn_size.x, btn_size.y, btnColor))
            {
                onClick.Invoke();
            }
            GUILayout.EndArea();
        }
    }
}
#endif