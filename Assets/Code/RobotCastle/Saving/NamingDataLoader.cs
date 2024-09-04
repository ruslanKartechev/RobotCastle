using System.IO;
using Newtonsoft.Json;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Saving
{
    public class NamingDataLoader
    {
        public const string FileName = "NamingConfig";
        public const string FileNameFormat = "NamingConfig.json";

        
        public static void Load()
        {
            var text = UnityEngine.Resources.Load<TextAsset>(FileName);
            if (text == null)
            {
                CLog.LogError("NamingData.FileName Does not exist!");
                return;
            }
            var data = JsonConvert.DeserializeObject<NamingData>(text.text);
            if (data == null)
            {
                CLog.LogError("NamingData could not be loaded");
                return;
            }
            NamingData.Set(data);
        }
        
#if UNITY_EDITOR
        public static string GetPathToFile()
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path = Path.Join(path, FileNameFormat);
            return path;
        }
        
        public static void CreateIfNone()
        {
            var path = GetPathToFile();
            if (File.Exists(path) == false)
            {
                CLog.LogWhite($"{path} File Does not exist. Creating new.");
                var data = NamingData.GetSample();
                var str = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path,str);
            }
        }

        public static void ForceWriteNew()
        {
            var path = GetPathToFile();
            var data = NamingData.GetSample();
            var str = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path,str);
        }
#endif
        
    }
}