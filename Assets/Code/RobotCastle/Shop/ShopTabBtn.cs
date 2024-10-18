using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class ShopTabBtn : MyButton
    {
        [SerializeField] private GameObject _highlight;

        public int Id { get; set; }
        
        public void SetPicked()
        {
            gameObject.SetActive(true);
            _highlight.SetActive(true);
            _btn.interactable = false;

        }

        public void SetNotPicked()
        {
            _highlight.SetActive(false);
            _btn.interactable = true;
        }

    }
}