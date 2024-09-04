using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IItemView
    {
        ItemData Data { get; set; }
        Transform Transform { get; }
        
        void OnPicked();
        void OnPut();
        void OnDroppedBack();
        void OnMerged();
        void UpdateViewToData(ItemData data);
        void Hide();
    }
}