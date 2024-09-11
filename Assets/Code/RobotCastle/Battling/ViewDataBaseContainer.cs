using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    [CreateAssetMenu(menuName = "SO/ViewDataBaseContainer", fileName = "ViewDataBaseContainer", order = 0)]
    public class ViewDataBaseContainer : ScriptableObject
    {
        private const string FileName = "View Data Base";
        public const string FileNameFormat = "View Data Base.json";

        private ViewDataBase _dataBase;

        public ViewDataBase DataBase => _dataBase;

        public void Load()
        {
            var text = UnityEngine.Resources.Load<TextAsset>(FileName);
            if (text == null)
            {
                CLog.LogError("FileName Does not exist!");
                return;
            }
            var data = JsonConvert.DeserializeObject<ViewDataBase>(text.text);
            if (data == null)
            {
                CLog.LogError("UnitsDataBase could not be loaded");
                return;
            }
            _dataBase = data;
        }
        
        
        
        
#if UNITY_EDITOR
        public static string GetPathToFile()
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path = Path.Join(path, FileNameFormat);
            return path;
        }

        [ContextMenu("CreateNewFile")]
        public void CreateNewFile()
        {
           Load();
           if (_dataBase == null)
           {
               CLog.LogRed("DABASE IS NULL");
               return;
           }
           
           _dataBase.ItemInfo = new Dictionary<string, ItemInfo>(50);
           var ids = new List<string>(50);
           var items = new List<string>() {
               "sword", "armor", "staff", "bow", "book_xp"
           };
           var heroes = new List<string>() {
               "shelda", "evan", "priya", "aramis", "daniel", "leonhard", "hansi", "asiaq", "maiu", "mara", "joi"
           };
           ids.AddRange(items);
           ids.AddRange(heroes);
           
           foreach (var id in ids)
           {
               var prefab = "prefabs/";
               var maxlvl = 0;
               if (heroes.Contains(id))
               {
                   maxlvl = 7;
                   prefab += "units/unit_";
               }
               else if (items.Contains(id))
               {
                   maxlvl = 3;
                   prefab += "units_items/item_";
               }
               var info = new ItemInfo()
               {
                   MaxMergeLevel = maxlvl,
                   Prefab = prefab + id,
                   Icon = $"sprites/icon_{id}"
               };
               _dataBase.ItemInfo.Add(id, info);
           }
           var str = JsonConvert.SerializeObject(_dataBase, Formatting.Indented);
           File.WriteAllText(GetPathToFile(),str);
        } 
        
        private void OnValidate()
        {
            var path = GetPathToFile();
            if (File.Exists(path) == false)
            {
                // CreateNewFile(path);
            }
        }
   
#endif

    }
}