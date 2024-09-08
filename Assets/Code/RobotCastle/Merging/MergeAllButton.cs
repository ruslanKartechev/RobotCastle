using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeAllButton : MonoBehaviour
    {
        [SerializeField] private ScaleWorldButton _worldButton;

        private void OnEnable()
        {
            _worldButton.OnClicked += Call;
        }

        private void Call()
        {
            ServiceLocator.Get<MergeManager>().MergeAll();
        }
    }
}