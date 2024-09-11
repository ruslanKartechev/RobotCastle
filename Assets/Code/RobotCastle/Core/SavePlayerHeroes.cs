using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class SavePlayerHeroes
    {
        public List<HeroSave> heroSaves = new List<HeroSave>(100);
        private Dictionary<string, int> _idIndexMap = new Dictionary<string, int>(100);

        public void Init()
        {
            _idIndexMap.Clear();
            CLog.Log($"[HeroSaves] Init with {heroSaves.Count} hero saves");
            for (var i = 0; i < heroSaves.Count; i++)
            {
                var save = heroSaves[i];
                _idIndexMap.Add(save.id, i);
            }
        }
        
        
        public HeroSave GetSave(string id)
        {
            if (_idIndexMap.ContainsKey(id))
            {
                return heroSaves[_idIndexMap[id]];
            }
            CLog.Log($"[HeroSaves] Did not contain {id}. Adding as new");
            var save = new HeroSave() { id = id };
            heroSaves.Add(save);
            _idIndexMap.Add(id, heroSaves.Count-1);
            return save;
        }
    }
}