using System.Collections;
using MAXHelper;
using RobotCastle.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobotCastle.Core
{
    [DefaultExecutionOrder(-1)]
    public class FirstSceneLoader : MonoBehaviour
    {
        [SerializeField] private string _firstScene;
        [SerializeField] private SceneLoaderUI _loadingUI;
        [SerializeField] private float _loadingConstTime = 1f;
        
        private void Start()
        {
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            yield return null;
            yield return null;
            while(!AdsManager.Exist || !AdsManager.Ready())
                yield return null;
            _loadingUI.Begin();
            yield return new WaitForSeconds(_loadingConstTime);
            SceneManager.LoadScene(_firstScene);
        }
    }
}