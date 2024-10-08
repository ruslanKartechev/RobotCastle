using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class DifficultyTierUI : MonoBehaviour
    {
        public SleepDev.Inventory.Item item;
        public List<GameObject> lockedObjects;
        public TextMeshProUGUI txtMultiplier;

        public void SetInteractable(bool can)
        {
            item.IsAllowedToPick = can;
        }

        public void SetLocked(bool locked)
        {
            foreach (var go in lockedObjects)
                go.SetActive(locked);
        }

        public void SetPercentMultiplier(float multiplier)
        {
            txtMultiplier.text = $"{Mathf.RoundToInt(multiplier * 100)}%";
        }
    }
}