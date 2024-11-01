using System;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.UI
{
    public class ExitBattleUI : MonoBehaviour
    {
        [SerializeField] private MyButton _closeBtn;
        [SerializeField] private MyButton _lvlExitBtn;
        
        public void Show(Action callback)
        {
            Time.timeScale = 0f;
            gameObject.SetActive(true);
            _closeBtn.AddMainCallback(Close);
            _lvlExitBtn.AddMainCallback(ExitLevel);
        }

        private void ExitLevel()
        {
            Time.timeScale = 1f;
            ServiceLocator.Get<SceneLoader>().LoadMainMenu();
        }

        private void Close()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
        
    }
}