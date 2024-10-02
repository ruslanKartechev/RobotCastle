using Newtonsoft.Json;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Data
{
    [CreateAssetMenu(menuName = "SO/XpDatabaseContainer", fileName = "XpDatabaseContainer", order = 0)]
    public class XpDatabaseContainer : ScriptableObject
    {
        public const string FileName = "Xp Database";
        
        [SerializeField]  private XpDatabase _database;

        public XpDatabase Database => _database;

        public void Load()
        {
            var text = UnityEngine.Resources.Load<TextAsset>(FileName);
            if (text == null)
            {
                CLog.LogError($"FileName {FileName} Does not exist!");
                return;
            }
            var data = JsonConvert.DeserializeObject<XpDatabase>(text.text);
            if (data == null)
            {
                CLog.LogError("UnitsDataBase could not be loaded");
                return;
            }
            _database = data;
        }
        
        
    }
}