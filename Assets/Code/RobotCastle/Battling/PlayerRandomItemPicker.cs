using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class PlayerRandomItemPicker : MonoBehaviour, IPlayerSummonItemPicker
    {
        [SerializeField] private WeightedList<CoreItemData> _possibleItems;

        public CoreItemData GetNext()
        {
            return _possibleItems.Random(true);
        }
        
    }
}