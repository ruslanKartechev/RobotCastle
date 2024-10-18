using System.IO;
using Newtonsoft.Json;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Shop
{
    [CreateAssetMenu(menuName = "SO/Shop DataBase Container", fileName = "Shop DataBase Container", order = 20)]
    public class ShopDataBaseContainer : ScriptableObject
    {
        public const string FileName = "SHOP Database";

        public ShopDataBase dataBaseEDITOR;
        
        public ShopDataBase dataBase => _dataBase;
        
        private ShopDataBase _dataBase;

        public void Load()
        {
            var text = UnityEngine.Resources.Load<TextAsset>(FileName);
            if (text == null)
            {
                CLog.LogError($"FileName {FileName} Does not exist!");
                return;
            }
            var data = JsonConvert.DeserializeObject<ShopDataBase>(text.text);
            if (data == null)
            {
                CLog.LogError("UnitsDataBase could not be loaded");
                return;
            }
            _dataBase = data;
        }
        
#if UNITY_EDITOR
        
        [ContextMenu("Save ")]
        public void Save()
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path = Path.Join(path, $"{FileName}.json");
            var str = JsonConvert.SerializeObject(dataBaseEDITOR, Formatting.Indented);
            File.WriteAllText(path, str);
            UnityEditor.AssetDatabase.Refresh();
        }
        
#endif
    }
}