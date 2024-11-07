using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public static class CLog
    {
        public static void LogException(string msg, string trace)
        {
            Debug.LogError($"[Exception] {msg}\n{trace}");
        }
        
        public static void LogError(string name, string msg)
        {
            Debug.LogError($"[{name}] {msg}");
        }
        
        public static void Log(string message)
        {
            Debug.Log(message);
        }
        
        public static void Log(string message, string color)
        {
            Debug.Log(message.Color(color));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void LogWHeader(string header, string message, string headerColor, string messageColor)
        {
            Debug.Log(Header(header, headerColor) + message.Color(messageColor));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static void LogWHeader(string header, string message, string color)
        {
            Debug.Log(Header(header, color) + message.Color(color));
        }
        
        public static void LogBlue(string message)
        {
            Debug.Log(message.Color("b"));
        }

        public static void LogGreen(string message)
        {
            Debug.Log(message.Color("g"));
        }

        public static void LogRed(string message)
        {
            Debug.Log(message.Color("r"));
        }
        
        public static void LogError(string message)
        {
            Debug.LogError("Error!! " + message);
        }
        
        public static void LogPink(string message)
        {
            Debug.Log(message.Color("p"));
        }

        public static void LogYellow(string message)
        {
            Debug.Log(message.Color("y"));
        }
        
        public static void LogWhite(string message)
        {
            Debug.Log(message.Color("w"));
        }

        public static string Header(string name, string color = "white")
        {
            return $"[{name.Color(color)}] ";
        }


        private static Dictionary<string, string> ColorKeys = new Dictionary<string, string>()
        {
            {"w", "FFFFFF"},
            {"black", "000000"},
            {"b", "blue"},
            {"c", "cyan"},
            {"g", "green"},
            {"y", "yellow"},
            {"r", "red"},
        };

        /// <summary>
        /// "r" - red, "y" - yellow, "b" - blue, "g" - green, "c" = cyan, "p" - pink
        /// "w" - white, "black" - black
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static string Color(this string message, string color)
        {
            #if !UNITY_EDITOR
            return message;
            #endif
            var colorPrefix = "FFFFFF";
            ColorKeys.TryGetValue(color, out colorPrefix);
            return $"<color={colorPrefix}>" + message + "</color>";
        }
    }
}