using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SleepDev
{
    public class SceneReloader : MonoBehaviour
    {
        public KeyCode reloadKey = KeyCode.R;
        public KeyCode pauseGameKey = KeyCode.Space;
        
        private void OnEnable()
        {
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            while (true)
            {
                if (Input.GetKeyDown(reloadKey))
                {
                    #if HAS_GAMECORE
                    GameCore.Core.GCon.PoolsManager.RecollectAll();
                    #endif
                    var scene = SceneManager.GetActiveScene();
                    SlowMotionManager.Inst?.SetNormalTime();
                    SceneManager.LoadScene(scene.name);
                }
                else if (Input.GetKeyDown(pauseGameKey))
                {
                    Debug.Break();
                }
                yield return null;
            }
        }
    }
}