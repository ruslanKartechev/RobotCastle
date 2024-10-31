using System.Collections.Generic;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class PlayerRandomItemPicker : MonoBehaviour, IPlayerSummonItemPicker
    {
    
        public CoreItemData GetNext()
        {
            var list = _options;
            if (_ignoreParty)
                list = _possibleItems;
            if (_providedCount < 3)
            {
                var it = 0;
                var itMax = 100;
                var result = list.Random();
                CLog.Log($"Hero lock case. Excluding some heroes from initial pool");
                while (HeroLock.Contains(result.id) && it < itMax)
                {
                    it++;
                    result = list.Random();
                }
                _providedCount++;
                return result;
            }

            _providedCount++;
            return list.Random();
        }

        
        private static readonly List<string> HeroLock = new() { "alberon", "asiaq", "priya"};

        [SerializeField] private bool _ignoreParty;
        [SerializeField] private WeightedList<CoreItemData> _possibleItems;
        private WeightedList<CoreItemData> _options;
        private int _providedCount = 0;
        
        private void Awake()
        {
            var save = DataHelpers.GetPlayerParty();
            _options = new WeightedList<CoreItemData>();
            _options.options = new List<WeightedList<CoreItemData>.Data<CoreItemData>>(save.heroesIds.Count);
            foreach (var id in save.heroesIds)
            {
                _options.options.Add(new WeightedList<CoreItemData>.Data<CoreItemData>(
                    new CoreItemData(0, id, "unit"), 1f));
            }
        }

   
    }
}