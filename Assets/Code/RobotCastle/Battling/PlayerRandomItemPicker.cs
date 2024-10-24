using System.Collections.Generic;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class PlayerRandomItemPicker : MonoBehaviour, IPlayerSummonItemPicker
    {
        [SerializeField] private bool _ignoreParty;
        [SerializeField] private WeightedList<CoreItemData> _possibleItems;
        private WeightedList<CoreItemData> _options;
        
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

        public CoreItemData GetNext()
        {
            if(_ignoreParty)
                return _possibleItems.Random(true);
            return _options.Random(true);
        }
        
    }
}