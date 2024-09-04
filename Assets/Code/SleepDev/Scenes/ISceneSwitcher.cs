using System;

namespace SleepDev
{
    public interface ISceneSwitcher
    {
        void OpenSceneAdditive(string name, Action<bool> onLoaded);
        void OpenScene(string name, Action<bool> onLoaded);
        void ReloadCurrent();
        
        void ClosePrevAdditiveScene();
        void CloseAllPrevAdditiveScene();
        
    }
}