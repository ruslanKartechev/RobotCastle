using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Data
{
    [CreateAssetMenu(menuName = "SO/ViewDataBaseContainer", fileName = "ViewDataBaseContainer", order = 0)]
    public class ViewDataBaseContainer : ScriptableObject
    {
        private const string FileNameDescriptions = "Descriptions Data Base";
        private const string FileNameFormatDescriptions = "Descriptions Data Base.json";
        
        private const string FileNameView = "View Data Base";
        private const string FileNameFormatView = "View Data Base.json";

        private DescriptionsDataBase _descriptionsDb;
        private ViewDataBase _viewDb;

        public DescriptionsDataBase descriptionsDb => _descriptionsDb;
        public ViewDataBase viewDb => _viewDb;
        

        public void Load()
        {
            LoadView();
            Descriptions();
        }

        private void Descriptions()
        {
            var text = UnityEngine.Resources.Load<TextAsset>(FileNameDescriptions);
            if (text == null)
            {
                CLog.LogError($"FileName {FileNameView} Does not exist!");
                return;
            }
            var data = JsonConvert.DeserializeObject<DescriptionsDataBase>(text.text);
            if (data == null)
            {
                CLog.LogError("UnitsDatDescriptionInfoaBase could not be loaded");
                return;
            }
            _descriptionsDb = data;
        }

        private void LoadView()
        {
            var text = UnityEngine.Resources.Load<TextAsset>(FileNameView);
            if (text == null)
            {
                CLog.LogError($"FileName {FileNameView} Does not exist!");
                return;
            }
            var data = JsonConvert.DeserializeObject<ViewDataBase>(text.text);
            if (data == null)
            {
                CLog.LogError("UnitsDataBase could not be loaded");
                return;
            }
            _viewDb = data;
        }
        
        
        
        
#if UNITY_EDITOR
        public static string GetPathToFile()
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path = Path.Join(path, FileNameFormatView);
            return path;
        }

        [ContextMenu("CreateNewFile")]
        public void CreateNewFile()
        {
           Load();
           if (_viewDb == null)
           {
               CLog.LogRed("DABASE IS NULL");
               return;
           }
           
           _viewDb.ItemInfo = new Dictionary<string, ItemInfo>(50);
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
               _viewDb.ItemInfo.Add(id, info);
           }
           var str = JsonConvert.SerializeObject(_viewDb, Formatting.Indented);
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