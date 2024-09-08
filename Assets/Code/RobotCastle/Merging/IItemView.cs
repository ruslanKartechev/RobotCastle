using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IItemView
    {
        ItemData itemData { get; set; }
        Transform Transform { get; }
        
        void OnPicked();
        void OnPut();
        void OnDroppedBack();
        void OnMerged();
        void UpdateViewToData(ItemData data = null);
        void Hide();
    }
}