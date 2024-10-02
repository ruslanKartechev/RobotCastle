using System;
using UnityEngine;

namespace RobotCastle.UI
{
    public interface IUIManager
    {
        Canvas ParentCanvas { get; set; }
        void Refresh();
        T Show<T>(string id, Action onClosed) where T : IScreenUI;
        T GetIfShown<T>(string id) where T : IScreenUI;
        void ShowScene(string id, Action onClosed);
        
        void AddAsShown<T>(string id, T obj) where T : IScreenUI;
        void OnClosed(string id);
    }
}