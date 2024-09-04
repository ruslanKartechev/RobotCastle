using UnityEngine;

namespace SleepDev
{
    public class OnStartHider : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}