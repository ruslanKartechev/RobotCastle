using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/ModifiersDataBase", fileName = "ModifiersDataBase", order = -10)]
    public class ModifiersDataBase : ScriptableObject
    {
        private const string ConfigFileName = "Modifiers Config"; 
        
        [SerializeField] private List<ModifierProvider> _spells;
        [SerializeField] private List<ModifierProvider> _otherModifiers;
        private readonly Dictionary<string, ModifierProvider> _map = new(100);
        private ModifiersConfig _config;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying == false)
            {
                var count1 = _spells.Count;
                var count2 = _otherModifiers.Count;
                _spells = _spells.Distinct().ToList();
                _otherModifiers = _otherModifiers.Distinct().ToList();
                if (_spells.Count != count1 || _otherModifiers.Count != count2)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
        #endif

        public void Load()
        {
            var file = Resources.Load<TextAsset>(ConfigFileName);
            if (file == null)
            {
                CLog.LogError("Cannot find config file");
                return;
            }
            _config = JsonConvert.DeserializeObject<ModifiersConfig>(file.text);
            if (_config == null)
            {
                CLog.LogError("Config is null. Failed to deserialize");
                return;
            }
            
            _map.Clear();
            foreach (var mod in _spells)
                _map.Add(mod.name, mod);
            foreach (var mod in _otherModifiers)
                _map.Add(mod.name, mod);
        }

        public ModifierProvider GetSpell(string id) => _map[id];

        public ModifierProvider GetModifier(string id)
        {
            if (_map.ContainsKey(id) == false)
            {
                CLog.LogRed($"NO {id} stored. Stored count: {_map.Count}");
                foreach (var (a,b) in _map)
                {
                    CLog.Log($"{a}");
                }
            }
            return _map[id];
        }

        public List<string> GetModifiersIdsForWeapon(string id, int lvl)
        {
            id += $"_lvl_{lvl+1}";
            var config = _config.weaponsModifiersMap[id];
            var modifiers = new List<string>(3);
            string modId;
            foreach (var dd in config.coreModifiers)
                modifiers.Add(dd);
            if (config.hasOptionalModifiers && config.optionalModifiers.Count > 0)
            {
                var mods = new List<string>(config.optionalModifiers);
                var count = UnityEngine.Random.Range(config.optionalModifiersCountMin, config.optionalModifiersCountMax);
                for (var i = 0; i < count && mods.Count > 0; i++)
                {
                    modId = mods.RemoveRandom();
                    modifiers.Add(modId);
                }
            }
            return modifiers;
        }
        
        
        public List<HeroWeaponData> GetWeaponsWithModifiers(List<CoreItemData> items)
        {
            var weaponData = new List<HeroWeaponData>(items.Count);
            foreach (var it in items)
            {
                var modifiers = GetModifiersIdsForWeapon(it.id, it.level);
                weaponData.Add(new HeroWeaponData(it, modifiers));   
            }
            return weaponData;
        }
        
        public HeroWeaponData GetWeaponsWithModifiers(CoreItemData it)
        {
            var modifiers = GetModifiersIdsForWeapon(it.id, it.level);
            return new HeroWeaponData(it, modifiers);
        }

    }
}