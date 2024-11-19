using System.Collections;
using RobotCastle.Data;
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
        private string _nextScene;


        public void LoadMainMenu()
        {
            StopLoading();
            RefreshAndSave();
            _nextScene = GlobalConfig.SceneMainMenu;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(GlobalConfig.SceneLoading);
        }

        public void LoadBattleScene()
        {
            StopLoading();
            RefreshAndSave();
            _nextScene = GlobalConfig.SceneBattle;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(GlobalConfig.SceneLoading);
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine(Waiting());
        }

        private IEnumerator Waiting()
        {
            yield return null;
            yield return null;
            yield return null;
            LoadNext();
        }

        private void LoadNext()
        {
            SceneManager.LoadScene(_nextScene);
        }
        
        private void StopLoading()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private void RefreshAndSave()
        {
            ServiceLocator.Get<IUIManager>().Refresh();
            DataHelpers.SaveData();
        }

    }
}