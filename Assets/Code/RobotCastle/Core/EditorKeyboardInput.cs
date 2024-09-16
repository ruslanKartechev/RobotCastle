using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobotCastle.Core
{
    public class EditorKeyboardInput : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEngine.KeyCode _reloadKey;
        [SerializeField] private UnityEngine.KeyCode _pauseKey;
        
        
        private void Update()
        {
            if (Input.GetKeyDown(_pauseKey))
            {
                Debug.Break();
            }
            else if (Input.GetKeyDown(_reloadKey))
            {
                var scene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(scene);
            }
        }
#endif
    }
}