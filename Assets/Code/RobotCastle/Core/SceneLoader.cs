using System.Collections;
using RobotCastle.Saving;
using RobotCastle.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobotCastle.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Create()
        {
            var go = new GameObject("scene_loader");
            var me = go.AddComponent<SceneLoader>();
            DontDestroyOnLoad(go);
            return me;
        }
        
        private Coroutine _working;
        

        public void LoadMainMenu()
        {
            StopLoading();
            _working = StartCoroutine(LoadingMainMenu());
        }

        public void LoadBattleScene()
        {
            StopLoading();
            _working = StartCoroutine(LoadingBattle());
        }

        private void StopLoading()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private void RefreshAndSave()
        {
            ServiceLocator.Get<IUIManager>().Refresh();
            ServiceLocator.Get<IDataSaver>().SaveAll();
        }

        private IEnumerator LoadingMainMenu()
        {
            yield return SceneManager.LoadSceneAsync(GlobalConfig.SceneLoading);
            RefreshAndSave();
            yield return SceneManager.LoadSceneAsync(GlobalConfig.SceneMainMenu);
        }

        private IEnumerator LoadingBattle()
        {
            yield return SceneManager.LoadSceneAsync(GlobalConfig.SceneLoading);
            RefreshAndSave();
            yield return SceneManager.LoadSceneAsync(GlobalConfig.SceneBattle);
        }
        
    }
}