using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class SortAllButton : MonoBehaviour
    {
        [SerializeField] private ScaleWorldButton _worldButton;

        private void OnEnable()
        {
            _worldButton.OnClicked += Call;
        }

        private void Call()
        {
            ServiceLocator.Get<MergeManager>().SortAll();
        }
    }
}