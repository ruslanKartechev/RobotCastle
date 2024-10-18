using UnityEngine;

namespace RobotCastle.UI
{
    public interface IItemDescriptionProvider
    {
        string GetIdForUI();
        GameObject GetGameObject();
        Vector3 WorldPosition { get; }
    }
}