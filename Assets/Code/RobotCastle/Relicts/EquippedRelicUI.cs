using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RobotCastle.Relics
{
    public class EquippedRelicUI : RelicItemUI
    { 
        [SerializeField] private RelicCellUI _cell;
        [SerializeField] private List<GameObject> _lockedGo;
        [SerializeField] private List<GameObject> _unLockedGo;
        [SerializeField] private TextMeshProUGUI _textLevelToUnlock;

        public bool IsUnlocked { get; set; }
        
        public void SetUnlockLevel(int level)
        {
            _textLevelToUnlock.text = (level + 1).ToString();
        }

        public void SetLocked()
        {
            IsUnlocked = false;
            _cell.IsAvailable = false;
            foreach (var go in _lockedGo)
                go.SetActive(true);
            foreach (var go in _unLockedGo)
                go.SetActive(false);
        }

        public void SetUnlocked()
        {
            IsUnlocked = true;
            _cell.IsAvailable = true;
            foreach (var go in _lockedGo)
                go.SetActive(false);
            foreach (var go in _unLockedGo)
                go.SetActive(true);
        }
        
    }
}