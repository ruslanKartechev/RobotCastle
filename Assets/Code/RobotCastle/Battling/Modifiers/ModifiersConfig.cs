using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public class ModifiersConfig
    {
        public Dictionary<string, ModifierData> weaponsModifiersMap = new();

        public ModifiersConfig()
        {
            weaponsModifiersMap = new Dictionary<string, ModifierData>(10);
        }

        public ModifiersConfig(ModifiersConfig other)
        {
            weaponsModifiersMap = new Dictionary<string, ModifierData>(other.weaponsModifiersMap.Count);
            foreach (var (id, data) in other.weaponsModifiersMap)
            {
                weaponsModifiersMap.Add(id, new ModifierData(data));
            }
        }

        [System.Serializable]
        public class ModifierData
        {
            public List<string> coreModifiers;
            public List<string> optionalModifiers;
            public bool hasOptionalModifiers;
            public int optionalModifiersCountMin;
            public int optionalModifiersCountMax;
            
            public ModifierData(){}

            public ModifierData(ModifierData other)
            {
                coreModifiers = new(other.coreModifiers);
                optionalModifiers = new (other.optionalModifiers);
                hasOptionalModifiers = other.hasOptionalModifiers;
                optionalModifiersCountMin = other.optionalModifiersCountMin;
                optionalModifiersCountMax = other.optionalModifiersCountMax;
            }
        }
    }
}