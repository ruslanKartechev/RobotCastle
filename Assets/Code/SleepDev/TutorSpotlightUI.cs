using UnityEngine;

namespace SleepDev
{
    public class TutorSpotlightUI : MonoBehaviour
    {
        [SerializeField] private GameObject _go;
        [SerializeField] private RectTransform _spotlight;
        [SerializeField] private float _defaultSize = 200;

        public void SetSize(float size)
        {
            _spotlight.sizeDelta = new Vector2(size, size);
        }

        public void SetDefaultSize()
        {
            SetSize(_defaultSize);
        }

        public void Show()
        {
            _spotlight.gameObject.SetActive(true);
            _go.SetActive(true);
        }

        public void HideAll()
        {
            _go.SetActive(false);
        }

        public void HideSelf()
        {
            _spotlight.gameObject.SetActive(false);
        }

        public void SetPosition(Vector3 worldPos)
        {
            _spotlight.position = worldPos;
        }
    }
}