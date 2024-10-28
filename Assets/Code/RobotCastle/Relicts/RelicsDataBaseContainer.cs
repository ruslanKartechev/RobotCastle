using System;
using Newtonsoft.Json;
using UnityEngine;

namespace RobotCastle.Relicts
{
    [CreateAssetMenu(menuName = "SO/Relicts DataBase Container", fileName = "Relicts DataBase Container", order = 300)]
    public class RelicsDataBaseContainer : ScriptableObject
    {
        public const string FileName = "Relicts DataBase";
        
        public RelicsDataBase database { get; private set; }

        public void Load()
        {
            var asset = Resources.Load<TextAsset>(FileName);
            if (asset == null)
            {
                Debug.LogError($"[RelictsDataBaseContainer] Asset {FileName} couldn't be loaded. error !");
                return;
            }
            var obj = JsonConvert.DeserializeObject<RelicsDataBase>(asset.text);
            if (obj == null)
            {
                Debug.LogError($"[RelictsDataBaseContainer] Failed to deserialized");
                return;
            }
            database = obj;
        }
    }
}