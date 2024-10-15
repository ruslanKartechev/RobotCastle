using System.Collections.Generic;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class BattleStartLevelData : IBattleStartData
    {
        public List<CoreItemData> StartItems => _startItems;
        public int StartMoney => _startMoney;
        
        [SerializeField] private int _startMoney = 9;
        [SerializeField] private List<CoreItemData> _startItems;

        public void AddMoney(int money)
        {
            _startMoney += money;
        }

        public void AddStartItem(CoreItemData item)
        {
            _startItems.Add(item);
        }
    }
}