using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Castle.Core.Internal;
using Newtonsoft.Json;
using SleepDev;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RobotCastle.Data
{
    public class TableReader : MonoBehaviour
    {
        public const string IdHP = "HP";
        public const string IdSP = "SP";
        public const string IdATK = "ATK";
        public const string IdATKSpeed = "ATK Speed";
        public const string IdMoveSpeed = "Move Speed";
        public const string IdPhysCritDamage = "Phys Crit Damage";
        public const string IdSPCritDamage = "Spell Crit Damage";
        public const string IdPhysCritChance = "Phys Crit Chance";
        public const string IdSPCritChance = "Spell Crit Chance";
        public const string IdPhysDef = "Phys DEF";
        public const string IdSPDef = "Spell DEF";
        public const string IdHpDrainPhys = "Phys HP Drain";
        public const string IdHpDrainSP = "Spell HP Drain";
        public const string IdEvasion = "Evasion";
        private const int MaxLvl = 20;
        private const int MaxMergeLvl = 7;

        [SerializeField] private List<TextAsset> _configFiles;
        [SerializeField] private List<string> _tableFiles;
        [Space(20)]
        [SerializeField] private string _folderPath;
        [SerializeField] private string _fileName;
        [SerializeField] private bool _attachFormat;
        [SerializeField] private string _format;
        [SerializeField] private HeroStats _heroStats;

        public void ReplaceAllConfigFromTables()
        {
#if UNITY_EDITOR
            foreach (var asset in _configFiles)
            {
                var id = asset.name;
                var heroConfig = JsonConvert.DeserializeObject<HeroInfo>(asset.text);
                if (heroConfig == null)
                {
                    CLog.Log($"Couldn't deserialize {id}");
                    continue;
                }
                var stats = ReadStatsForHero(id);
                if (stats == null)
                {
                    // CLog.Log($"Couldn't read table file for: {id}");
                    continue;
                }
                heroConfig.stats = stats;

                var path = AssetDatabase.GetAssetPath(asset);
                if (path.IsNullOrEmpty())
                {
                    CLog.Log($"Couldn't get file path: {asset.name}");
                    continue;
                }
                var pathToAssets = Application.streamingAssetsPath;
                pathToAssets = pathToAssets.Split("Assets")[0];
                path = pathToAssets + path;
                CLog.LogGreen($"Overriding at path: {path}");
                var serialized = JsonConvert.SerializeObject(heroConfig, Formatting.Indented);
                File.WriteAllText(path, serialized);
            }
#endif
        }

        public HeroStats ReadStatsForHero(string heroId)
        {
            var tableFileName = _tableFiles.Find(t => t.Contains(heroId));
            if (tableFileName == default)
            {
                CLog.Log($"Table File for: {heroId} wasn't found");
                return null;
            }
            var stats = ReadTableFile(tableFileName);
            return stats;
        }
        
        public void Read()
        {
            var stats = ReadTableFile(_fileName);
            _heroStats = stats;
        }
            
        public HeroStats ReadTableFile(string file)
        {
            var path = Path.Join(_folderPath, file);
            if(_attachFormat)
                path += $".{_format}";
            
            if (File.Exists(path) == false)
            {
                CLog.LogRed($"File doesn't exist: {path} ");
                return null;
            }
            var reader = new StreamReader(path);
            var delimiter = '\t';
            var columns = reader.ReadLine().Split(delimiter);
            var msg = "";
            foreach (var hh in columns)
                msg += hh + " ";
            var heroStats = new HeroStats();
            for (var rowInd = 1; reader.Peek() > 0; rowInd++)
            {
                columns = reader.ReadLine().Split(delimiter);
                var header = columns[0];
                // CLog.Log($"Row  header: {header}");
                switch (header)
                {
                    case IdHP:
                        heroStats.health = new List<float>(MaxLvl);
                        if (!ReadValues(columns, heroStats.health, MaxLvl))
                            goto EXIT_NULL;
                        break;
                    case IdSP:
                        heroStats.spellPower = new List<float>(MaxLvl);
                        if (!ReadValues(columns, heroStats.spellPower, MaxLvl))
                            goto EXIT_NULL;
                        break;
                    case IdATK:
                        heroStats.attack = new List<float>(MaxLvl);
                        if (!ReadValues(columns, heroStats.attack, MaxLvl))
                            goto EXIT_NULL;
                        break;
                    case IdATKSpeed:
                        heroStats.attackSpeed = new List<float>(MaxMergeLvl);
                        if (!ReadValues(columns, heroStats.attackSpeed, MaxMergeLvl, 1))
                            goto EXIT_NULL;
                        break;
                    case IdMoveSpeed:
                        heroStats.moveSpeed = new List<float>(1);
                        if (!ReadValues(columns, heroStats.moveSpeed, 1))
                            goto EXIT_NULL;
                        break;
                    case IdPhysCritDamage:
                        heroStats.physicalCritDamage = new List<float>(1);
                        if (!ReadValues(columns, heroStats.physicalCritDamage, 1, 0.01f))
                            goto EXIT_NULL;
                        break;
                    case IdSPCritDamage:
                        heroStats.magicalCritDamage = new List<float>(1);
                        if (!ReadValues(columns, heroStats.magicalCritDamage, 1, 0.01f))
                            goto EXIT_NULL;
                        break;
                    case IdPhysCritChance:
                        heroStats.physicalCritChance = new List<float>(1);
                        if (!ReadValues(columns, heroStats.physicalCritChance, 1, 0.01f))
                            goto EXIT_NULL;
                        break;
                    case IdSPCritChance:
                        heroStats.magicalCritChance = new List<float>(1);
                        if (!ReadValues(columns, heroStats.magicalCritChance, 1, 0.01f))
                            goto EXIT_NULL;
                        break;
                    case IdPhysDef:
                        heroStats.physicalResist = new List<float>(1);
                        if (!ReadValues(columns, heroStats.physicalResist, 1))
                            goto EXIT_NULL;
                        break;
                    case IdSPDef:
                        heroStats.magicalResist = new List<float>(1);
                        if (!ReadValues(columns, heroStats.magicalResist, 1))
                            goto EXIT_NULL;
                        break;
                    case IdHpDrainPhys:
                        heroStats.physHpDrain = new List<float>(1);
                        if (!ReadValues(columns, heroStats.physHpDrain, 1, 0.01f))
                            goto EXIT_NULL;
                        break;
                    case IdHpDrainSP:
                        heroStats.magicHpDrain = new List<float>(1);
                        if (!ReadValues(columns, heroStats.magicHpDrain, 1, 0.01f))
                            goto EXIT_NULL;
                        break;
                    case IdEvasion:
                        heroStats.evasion = new List<float>(1);
                        if (!ReadValues(columns, heroStats.evasion, 1, 0.01f))
                            goto EXIT_NULL;
                        break;
                    default:
                        CLog.Log($"Unknown stat: {header}");
                        break;
                        // goto EXIT_NULL;
                }
            }
            return heroStats;
            
            EXIT_NULL:
                return null;
        }

        private bool ReadValues(string[] columns, List<float> list, int maxCount, float multiplier = 1f)
        {
            var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            for (var lvl = 0; lvl < maxCount; lvl++)
            {
                var strVal = columns[lvl + 1];
                strVal = strVal.Trim(' ', '%');
                // strVal = strVal.Replace('.', ',');
                if (float.TryParse(strVal, style, culture, out var val))
                {
                    list.Add(val * multiplier);                                
                    // CLog.Log($"parsed: {val * multiplier}");
                }
                else
                {
                    CLog.LogError($"Cannot parse into float: __{strVal}__!!");
                    return false;
                }
            }
            return true;
        }
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(TableReader))]
    public class TableReaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var me = target as TableReader; 
            EU.Space();
            if (EU.BtnMidWide3("ReplaceConfigFromTables", EU.Gold))
                me.ReplaceAllConfigFromTables();
            EU.Space();
            if(EU.BtnMidWide2("Read To Debug", EU.Gold))
                me.Read();
        }
    }
    #endif
    
}