using UnityEngine;

namespace RobotCastle.Battling
{
    public class UIWorldPositionTracker : MonoBehaviour
    {
        private Transform _target;

        public void SetTarget(Transform point)
        {
            gameObject.SetActive(true);
            _target = point;
        }
        
        private void Update()
        {
            if (_target != null)
            {
                if(_target.gameObject.activeSelf == false)
                    gameObject.SetActive(false);
                transform.position = Camera.main.WorldToScreenPoint(_target.position);
            }
            else
                gameObject.SetActive(false);
        }
    }
}