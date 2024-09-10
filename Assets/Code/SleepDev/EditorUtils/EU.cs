using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
#if UNITY_EDITOR
    /// <summary>
    /// Useful editor methods for building editor classes
    /// </summary>
    public class EU
    {
        public const float button_small_width = 30;
        public const float button_mid_width = 60;
        public const float button_big_width = 90;
        public const float button_large_width = 120;

        public const float button_small_height = 30;
        public const float button_mid_height = 36;
        public const float button_big_height = 50;

        public const int font_size_small = 10;
        public const int font_size_normal = 12;
        public const int font_size_big = 15;
        public const int font_size_large = 18;
        public const int font_size_huge = 25;
        
        public const int space_small = 10;
        public const int space_mid = 20;
        public const int space_big = 30;
        public const int space_large = 40;

        public const int square_btn_size_small = 30;
        public const int square_btn_size_mid = 40;
        public const int square_btn_size_big = 50;
        public const int square_btn_size_large = 60;

        
        #region Colors

        private const float Alpha = 0.85f;
        public static Color White = new(255/255f, 255/255f, 255/255f, Alpha);      
        public static Color Black = new(0, 0, 0, Alpha);      
        /// Red
        public static Color Red = new(255/255f, 0, 0, Alpha);      
        /// Red
        public static Color LightCoral = new(240/255f, 128/255f, 128/255f, Alpha);        
        /// Red
        public static Color Crimson = new (220/255f, 20/255f, 60/255f, Alpha);        
        /// Red
        public static Color FireBrick = new (178/255f, 34/255f, 34/255f, Alpha);        
        /// Pink
        public static Color Pink = new (255/255f, 192/255f, 203/255f, Alpha);        
        /// Pink
        public static Color HotPink = new (255/255f, 105/255f, 180/255f, Alpha);
        /// Pink
        public static Color DeepPink = new (255/255f, 20/255f, 147/255f, Alpha);
        
        /// Yellow
        public static Color DarkOrange = new (255/255f, 140/255f, 0, Alpha);
        /// Yellow
        public static Color Orange = new (255/255f, 165/255f, 0, Alpha);
        /// Yellow
        public static Color Gold = new (255/255f, 215/255f, 0, Alpha);
        /// Yellow
        public static Color LightYellow = new (255/255f, 255/255f, 224/255f, Alpha);
        /// Yellow
        public static Color Moccasin = new (255/255f, 228/255f, 181/255f, Alpha);
        /// Yellow
        public static Color PeachPuff = new (255/255f, 218/255f, 185/255f, Alpha);
        
        /// Purple
        public static Color Lavender = new (230/255f, 230/255f, 250/255f, Alpha);
        /// Purple
        public static Color Plum = new (221/255f, 160/255f, 221/255f, Alpha);
        /// Purple
        public static Color Violent = new (238/255f, 130/255f, 238/255f, Alpha);
        /// Purple
        public static Color Fuchsia = new (255/255f, 0, 255/255f, Alpha);
        /// Purple
        public static Color MediumOrchid = new (186/255f, 85/255f, 211/255f, Alpha);
        /// Purple
        public static Color MediumPurple = new (147/255f, 112/255f, 219/255f, Alpha);
        /// Purple
        public static Color RebeccaPurple = new (102, 51/255f, 153/255f, Alpha);
        /// Purple
        public static Color Purple = new (128/255f, 0, 128/255f, Alpha);
        /// Purple
        public static Color MediumSlateBlue = new (123/255f, 104/255f, 238/255f, Alpha);
        
        /// Green
        public static Color GreenYellow = new (173/255f, 255/255f, 47/255f, Alpha);
        /// Green
        public static Color Chartreuse = new (127/255f, 255/255f, 0, Alpha);
        /// Green
        public static Color Lime = new (0, 255/255f, 0, Alpha);
        /// Green
        public static Color SpringGreen = new (0, 1f, 127/255f, Alpha);
        /// Green
        public static Color ForestGreen = new (34/255f, 139/255f, 34/255f, Alpha);
        /// Green
        public static Color DarkGreen = new (0, 100/255f, 0, Alpha);
        /// Green
        public static Color MediumAquamarine = new (102/255f, 205/255f, 170/255f, Alpha);
        /// Green
        public static Color LightSeaGreen = new (32/255f, 178/255f, 170/255f, Alpha);
        /// Green
        public static Color DarkCyan = new (0, 139/255f, 139/255f, Alpha);
        
        /// Blue
        public static Color Aqua = new (0, 255/255f, 255/255f, Alpha);
        /// Blue
        public static Color PaleTurquoise = new (175/255f, 238/255f, 238/255f, Alpha);
        /// Blue
        public static Color Turquoise = new (64/255f, 224/255f, 208/255f, Alpha);
        /// Blue
        public static Color SteelBlue = new (70/255f, 130/255f, 180/255f, Alpha);
        /// Blue
        public static Color DeepSkyBlue = new (0, 191/255f, 255/255f, Alpha);
        /// Blue
        public static Color RoyalBlue = new (65/255f, 105/255f, 225/255f, Alpha);
        /// Blue
        public static Color MediumBlue = new (0, 0, 205/255f, Alpha);
        /// Blue
        public static Color Navy = new (0, 0, 128/255f, Alpha);
        #endregion

        
#region Public Buttons

        public static bool Btn(string label, float width, float height, Color color, int fontSize = -1)
        {
            var style = GetButtonStyle(width, height);
            style.fontStyle = FontStyle.Bold;
            if (fontSize > 0)
                style.fontSize = fontSize;
            var prevColor = GUI.color;
            GUI.color = color;
            SetButtonTextColor(style,color);
            var clicked = GUILayout.Button(label, style);
            GUI.color = prevColor;
            return clicked;
        }

        /// <summary>
        /// Small Square button. Use this with no text or one character
        /// </summary>
        public static bool BtnSmallSquare(string label, Color color, int fontSize = -1)
        {
            return Button(label, button_small_width, button_small_height, color, fontSize);
        }
        
        public static bool BtnSquare(string label, Color color, int squareSide, int fontSize = -1)
        {
            return Button(label, squareSide, squareSide, color, fontSize);
        }
        /// <summary>
        /// Normal sized button, suitable for 1-word text
        /// </summary>
        public static bool BtnMid1(string label, Color color, int fontSize = -1)
        {
            return Button(label, button_mid_width, button_mid_height, color, fontSize);
        }
        
        public static bool BtnMid2(string label, Color color, int fontSize = -1)
        {
            return Button(label, button_mid_width * 1.25f, button_mid_height, color, fontSize);
        }
        
        public static bool BtnMidSmallHeight(string label, Color color, int fontSize = -1)
        {
            return Button(label, button_mid_width, button_small_height, color, fontSize);
        }
        
        /// <summary>
        /// Big Width, mid width
        /// </summary>
        public static bool BtnMidWide(string label, Color color, int fontSize = -1)
        {
            return Button(label, button_big_width, button_mid_height, color, fontSize);
        }
        
        public static bool BtnMidWide2(string label, Color color, int fontSize = -1)
        {
            return Button(label, button_large_width, button_mid_height, color, fontSize);
        }
        
        public static bool BtnMidWide3(string label, Color color, int fontSize = -1)
        {
            return Button(label, 160, button_mid_height, color, fontSize);
        }

        /// <summary>
        ///  Big Width, Big Height
        /// </summary>
        public static bool BtnBigWide(string label, Color color, int fontSize = -1)
        {
            return Button(label, button_big_width, button_big_height, color, fontSize);
        }
        
        /// <summary>
        ///  Large Width, Big Height
        /// </summary>
        public static bool BtnLargeWide(string label, Color color)
        {
            return Button(label, button_large_width, button_big_height, color);
        }

        public static bool Button(string label, float width, float height, Color color, int fontSize = -1)
        {
            var style = GetButtonStyle(width, height);
            style.fontStyle = FontStyle.Bold;
            if (fontSize > 0)
                style.fontSize = fontSize;
            var prevColor = GUI.color;
            GUI.color = color;
            SetButtonTextColor(style,color);
            var clicked = GUILayout.Button(label, style);
            GUI.color = prevColor;
            return clicked;
        }
#endregion


#region Public Labels
        public static void Label(string text, char align = 'c', bool bold = true)
        {
            var style = GetLabelStyle(align, bold);
            GUILayout.Label(text, style);
        }

        public static void Label(string text, Color color, char align = 'c', bool bold = true)
        {
            var prevColor = GUI.color;
            GUI.color = color;
            Label(text, align, bold);
            GUI.color = prevColor;
        }
        
        public static void Label(string text, Color color, int fontSize, char align = 'c', bool bold = true)
        {
            var prevColor = GUI.color;
            GUI.color = color;
            Label(text, fontSize, align, bold);
            GUI.color = prevColor;
        }
        
        public static void Label(string text, Color color, int fontSize, int width, char align = 'c', bool bold = true)
        {
            var prevColor = GUI.color;
            GUI.color = color;
            var style = GetLabelStyle(align, bold, fontSize);
            GUILayout.Label(text, style, GUILayout.Width(width));
            GUI.color = prevColor;
        }

                
        public static void Label(string text, int fontSize = font_size_normal, char align = 'c', bool bold = true)
        {
            var style = GetLabelStyle(align, bold, fontSize);
            GUILayout.Label(text, style);
        }
        
        public static void LabelRect(Rect rect, string label, int fontSize, Color color, char align, bool bold)
        {
            var prevColor = GUI.color;
            GUI.color = color;
            var style = GetLabelStyle(align, bold, fontSize);
            EditorGUI.LabelField(rect, label, style);
            GUI.color = prevColor;
        }        
        
        public static void LabelRect(Rect rect, string label, GUIStyle style)
        {
            EditorGUI.LabelField(rect, label, style);
        }

#endregion


        #region Spaces
        public static void Space(int size = space_small)
        {
            GUILayout.Space(size);
        }

        #endregion


        #region Button With Label

        public static bool ButtonWithLabelSmall(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white);
        }
        
        public static bool ButtonWithLabelMidSize(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white, square_btn_size_mid);
        }

        public static bool ButtonWithLabelBig(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white, square_btn_size_big);
        }
        
        public static bool ButtonWithLabelLarge(string buttonText, string labelText, Color buttonColor)
        {
            return ButtonWithLabel(buttonText, labelText, buttonColor, Color.white, square_btn_size_large);
        }
        
        public static bool ButtonWithLabel(string buttonText, string labelText, 
                            Color buttonColor, Color labelColor,
                            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('l', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            GUI.color = buttonColor;
            SetButtonTextColor(buttonStyle, buttonColor);
            var pressed = GUILayout.Button(buttonText, buttonStyle);
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);
            GUILayout.EndHorizontal();
            GUI.color = prevColor;
            return pressed;
        }

        public static bool LabelWithButton(string buttonText, string labelText, 
            Color buttonColor, Color labelColor,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('r', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            GUI.color = buttonColor;
            GUILayout.Label(labelText, labelStyle);
            SetButtonTextColor(buttonStyle, buttonColor);
            var pressed = GUILayout.Button(buttonText, buttonStyle);
            GUI.color = labelColor;
            GUILayout.EndHorizontal();
            GUI.color = prevColor;
            return pressed;
        }
        
        public static void TwoButtonAndLabel(string buttonText1, string buttonText2, string labelText, 
            Color buttonColor1, Color buttonColor2, Color labelColor,
            Action onClick1, Action onClick2,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);

            var labelStyle = GetLabelStyle('l', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            
            GUI.color = buttonColor1;
            SetButtonTextColor(buttonStyle, buttonColor1);
            if(GUILayout.Button(buttonText1, buttonStyle))
                onClick1?.Invoke();
            GUI.color = buttonColor2;
            SetButtonTextColor(buttonStyle, buttonColor2);
            if(GUILayout.Button(buttonText2, buttonStyle))
                onClick2?.Invoke();
            
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);

            GUILayout.EndHorizontal();
            GUI.color = prevColor;
        }
        
        public static void LabelAndTwoButton(string buttonText1, string buttonText2, string labelText, 
            Color buttonColor1, Color buttonColor2, Color labelColor,
            Action onClick1, Action onClick2,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('r', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);
            
            GUI.color = buttonColor1;
            SetButtonTextColor(buttonStyle, buttonColor1);
            if(GUILayout.Button(buttonText1, buttonStyle))
                onClick1?.Invoke();
            GUI.color = buttonColor2;
            SetButtonTextColor(buttonStyle, buttonColor2);
            if(GUILayout.Button(buttonText2, buttonStyle))
                onClick2?.Invoke();

            GUILayout.EndHorizontal();
            GUI.color = prevColor;
        }
        
        public static void ThreeButtonAndLabel(string buttonText1, string buttonText2,  string buttonText3, string labelText, 
            Color buttonColor1, Color buttonColor2, Color buttonColor3, Color labelColor,
            Action onClick1, Action onClick2, Action onClick3,
            int buttonSize = square_btn_size_small, int fontSize = font_size_normal)
        {
            var prevColor = GUI.color;
            var buttonStyle = GetButtonStyle(buttonSize, buttonSize);
            var labelStyle = GetLabelStyle('l', true, fontSize);
            labelStyle.fixedHeight = buttonSize;
            GUILayout.BeginHorizontal();
            
            GUI.color = buttonColor1;
            SetButtonTextColor(buttonStyle, buttonColor1);
            if(GUILayout.Button(buttonText1, buttonStyle))
                onClick1?.Invoke();
            GUI.color = buttonColor2;
            SetButtonTextColor(buttonStyle, buttonColor2);
            if(GUILayout.Button(buttonText2, buttonStyle))
                onClick2?.Invoke();
            GUI.color = buttonColor3;
            SetButtonTextColor(buttonStyle, buttonColor3);
            if(GUILayout.Button(buttonText3, buttonStyle))
                onClick3?.Invoke();
            
            GUI.color = labelColor;
            GUILayout.Label(labelText, labelStyle);

            GUILayout.EndHorizontal();
            GUI.color = prevColor;
        }

        private static void SetButtonTextColor(GUIStyle style, Color buttonColor)
        {
            var cl = (buttonColor.r + buttonColor.g + buttonColor.b) * buttonColor.a;
            if (cl > 50)
                style.normal.textColor = Black;
            else
                style.normal.textColor = Color.white;
        }
        #endregion
        
        
        #region Style Getters
        public static GUIStyle GetButtonStyle(float width, float height, int fontSize = font_size_normal)
        {
            var style = GetButtonStyle();
            style.fixedWidth = width;
            style.fixedHeight = height;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = fontSize;
            return style;
        }
        
        public static GUIStyle GetButtonStyle()
        {
            var style = new GUIStyle(GUI.skin.button);
            return style;
        }
        
        public static GUIStyle GetLabelStyle(char align, bool bold, int fontSize = font_size_normal)
        {
            var style = GetLabelStyle();
            switch (align)
            {
                case 'l':
                    style.alignment = TextAnchor.MiddleLeft;
                    break;
                case 'c':
                    style.alignment = TextAnchor.MiddleCenter;
                    break;
                case 'r':
                    style.alignment = TextAnchor.MiddleRight;
                    break;
            }
            if (bold)
                style.fontStyle = FontStyle.Bold;
            style.fontSize = fontSize;
            return style;    
        }
        
        public static GUIStyle GetLabelStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            return style;
        }    
        #endregion


        #region Coloumns

        public class EU_ColumnElement
        {
            public string content;
            public int fontSize;
            public Color color;
            public char align;
            public bool bold;

            public EU_ColumnElement(string content)
            {
                this.content = content;
            }
            
            public EU_ColumnElement(string content, int fontSize, Color color, char align = 'l', bool bold = true)
            {
                this.content = content;
                this.fontSize = fontSize;
                this.color = color;
                this.align = align;
                this.bold = bold;
            }

            public void CopyStyle(EU_ColumnElement source)
            {
                this.fontSize = source.fontSize;
                this.color = source.color;
                this.align = source.align;
                this.bold = source.bold;

            }
        }

        public class EU_Column
        {
            public List<EU_ColumnElement> elements;
            public EU_ColumnElement defaultElement;
            public Rect rect;
            
            public EU_Column(Rect rect)
            {
                this.rect = rect;
                elements = new List<EU_ColumnElement>();
            }
            
            public EU_Column(Rect rect, EU_ColumnElement defaultElement)
            {
                this.rect = rect;
                this.defaultElement = defaultElement;
                elements = new List<EU_ColumnElement>();
            }

            public void SetDefaultElement(EU_ColumnElement defaultElement, bool copyForOthers)
            {
                this.defaultElement = defaultElement;
                if (copyForOthers)
                {
                    foreach (var el in elements)
                        el.CopyStyle(defaultElement);
                }
            }

            public void AddElement(string content)
            {
                var el = new EU_ColumnElement(content);
                el.CopyStyle(defaultElement);
                AddElement(el);
            }

            public void AddElement(EU_ColumnElement element)
            {
                elements.Add(element);
            }

            public void Show()
            {
                GUILayout.BeginArea(rect);
                foreach (var el in elements)
                    Label(el.content, el.color, el.fontSize, el.align, el.bold);
                GUILayout.EndArea();
            }
        }

        public static bool Toggle(string label, int fontsize, Color color, float width, bool value)
        {
            var skin = new GUIStyle(GUI.skin.label);
            skin.alignment = TextAnchor.MiddleLeft;
            skin.fontSize = fontsize;
            var oldColor = GUI.color;
            GUILayout.BeginHorizontal();
            GUI.color = color;
            GUILayout.Label(label,skin, GUILayout.Width(width));
            GUI.color = oldColor;
            var val = EditorGUILayout.Toggle(value);
            GUILayout.EndHorizontal();
            return val;
        }
        
        public static string TextField(string label, int fontsize, Color color, float width, string value)
        {
            var skin = new GUIStyle(GUI.skin.label);
            skin.alignment = TextAnchor.MiddleLeft;
            skin.fontSize = fontsize;
            var oldColor = GUI.color;
            GUILayout.BeginHorizontal();
            GUI.color = color;
            GUILayout.Label(label,skin, GUILayout.Width(width));
            GUI.color = oldColor;
            var val = EditorGUILayout.TextField(value);
            GUILayout.EndHorizontal();
            return val;
        }

        public static UnityEngine.Object ObjectField(string label, int fontsize, Color color, float width, 
            UnityEngine.Object obj, Type objType)
        {
            var skin = new GUIStyle(GUI.skin.label);
            skin.alignment = TextAnchor.MiddleLeft;
            skin.fontSize = fontsize;
            var oldColor = GUI.color;
            GUILayout.BeginHorizontal();
            GUI.color = color;
            GUILayout.Label(label,skin, GUILayout.Width(width));
            GUI.color = oldColor;
            var val = EditorGUILayout.ObjectField(obj, objType);
            GUILayout.EndHorizontal();
            return val;
        }
        #endregion

        #region Utils
        public static Rect SetRectX(Rect rect, float x)
        {
            var rect2 = new Rect(rect);
            rect2.x = x;
            rect2.width -= x;
            return rect2;
        }

        #endregion
        
    }
    #endif
}