using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class MainMenuCamera : MonoBehaviour
    {
        [SerializeField] private Transform _movable;

        public void SetPoint(Transform point)
        {
            _movable.SetPositionAndRotation(point.position, point.rotation);
        }

    }
}