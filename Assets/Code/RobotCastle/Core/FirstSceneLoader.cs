using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobotCastle.Core
{
    [DefaultExecutionOrder(-1)]
    public class FirstSceneLoader : MonoBehaviour
    {
        [SerializeField] private string _firstScene;

        private void Start()
        {
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            yield return null;
            yield return null;
            yield return null;
            SceneManager.LoadScene(_firstScene);
        }
    }
}