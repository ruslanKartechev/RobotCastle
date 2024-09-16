using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IItemView
    {
        ItemData itemData { get; set; }
        Transform Transform { get; }
        
        void InitView(ItemData data);
        void UpdateViewToData(ItemData data = null);
        void Hide();
        
        void OnPicked();
        void OnPut();
        void OnDroppedBack();
        void OnMerged();

        void Rotate(Quaternion rotation, float time);
        void MoveToPoint(Transform endPoint, float time);
    }
    
}