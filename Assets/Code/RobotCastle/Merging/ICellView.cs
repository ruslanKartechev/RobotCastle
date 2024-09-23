using UnityEngine;

namespace RobotCastle.Merging
{
    public interface ICellView
    {
        int GridId { get; set; }
        Cell cell { get; set; }
        IItemView itemView { get; set; }
        void OnPicked();
        void OnPut();
        void OnDroppedBack();
        void SetHighlightForMerge(bool on, int type);
        void HighlightAsUnderCell(bool on);
        Transform ItemPoint { get; }        
        Vector3 WorldPosition { get; }
        Quaternion WorldRotation { get; }
    }
}