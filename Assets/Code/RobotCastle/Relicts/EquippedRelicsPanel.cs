using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Relicts
{
    public class EquippedRelicsPanel : MonoBehaviour
    {
        [SerializeField] private bool _autoInitOnEnable;
        [SerializeField] private List<EquippedRelicUI> _itemsUI;
        private List<RelicSave> _equippedItems;


        public void Init()
        {
            var saves = DataHelpers.GetPlayerData().relics;
            var availableSlots = saves.unlockedSlotsCount;
            var db = ServiceLocator.Get<RelicsDataBase>();
            for (var i = 0; i < _itemsUI.Count; i++)
            {
                _itemsUI[i].SetEmpty();
                if (i < availableSlots)
                {
                    _itemsUI[i].SetUnlocked();
                }
                else
                {
                    _itemsUI[i].SetUnlockLevel(db.playerLevelsToUnlockSlots[i]);
                    _itemsUI[i].SetLocked();
                }
            }
            _equippedItems = new List<RelicSave>();
            var slotInd = 0;
            for (var i = 0; i < saves.allRelics.Count && slotInd < availableSlots; i++)
            {
                var temp = saves.allRelics[i];
                if (temp.isEquipped)
                {
                    _equippedItems.Add(temp);
                    var ui = _itemsUI[slotInd];
                    var data = db.relicData[temp.core.id];
                    ui.SetDataAndIcon(data, temp, Resources.Load<Sprite>(data.icon));
                    slotInd++;
                }
            }
        }

        public bool HasRelicWithId(string id)
        {
            return _itemsUI.Find(t => 
                t != null && t.IsUnlocked && t.relicData != null && t.relicData.core.id == id) != default;
        }
        
        private void OnEnable()
        {
            if(_autoInitOnEnable)
                Init();
        }
        
    }
}