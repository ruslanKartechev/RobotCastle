using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/ModifiersDataBase", fileName = "ModifiersDataBase", order = -10)]
    public class ModifiersDataBase : ScriptableObject
    {
        [SerializeField] private List<ModifierProvider> _modifiers;
        [SerializeField] private List<ModifierProvider> _spells;
        
        private readonly Dictionary<string, ModifierProvider> _map = new(100);
   
        public void Load()
        {
            _map.Clear();
            foreach (var mod in _modifiers)
                _map.Add(mod.name, mod);
            foreach (var mod in _spells)
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
    }
}