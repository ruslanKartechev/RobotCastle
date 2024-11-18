using TMPro;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class ReturnItemButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private GameObject _go;
        
        public bool IsAbove()
        {
            return _rect.rect.Contains(Input.mousePosition);
        }
        
        public void SetMoney(int amount)
        {
            _costText.text = $"+{amount}";
        }

        public void Show() => _go.SetActive(true);

        public void Hide() => _go.SetActive(false);
    }
}