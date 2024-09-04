using System;

namespace RobotCastle.UI
{
    public interface IUIManager
    {
        void Refresh();
        T Show<T>(string id, Action onClosed) where T : IScreenUI;
        void ShowScene(string id, Action onClosed);
        
        void AddAsOpened<T>(string id, T obj) where T : IScreenUI;
        void OnClosed(string id);
    }
}