using UnityEngine;

namespace RobotCastle.UI
{
    public abstract class DescriptionUI : MonoBehaviour
    {
        public abstract void Show(GameObject source);
        public abstract void Hide();
        public virtual void OffNow() => gameObject.SetActive(false);
    }
}