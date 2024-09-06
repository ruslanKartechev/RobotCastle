using UnityEngine;

namespace RobotCastle.Merging
{
    public interface ICellView
    {
        Cell cell { get; set; }
        IItemView item { get; set; }
        void OnPicked();
        void OnPut();
        void OnDroppedBack();
        void SetHighlightForMerge(bool on, int type);
        void HighlightAsUnderCell(bool on);
        Transform ItemPoint { get; }        
    }
}