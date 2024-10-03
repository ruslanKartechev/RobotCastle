using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SleepDev.Inventory
{
    public class BasicItem : Item
    {
        [SerializeField] protected string _id;
        [SerializeField] protected Image _icon;
        [SerializeField] protected TextMeshProUGUI _countText;
        protected int _count;

        public override string Id
        {
            get => _id;
            set => _id = value;
        }

        public override void Pick()
        {
            IsPicked = true;
        }

        public override void Unpick()
        {
            IsPicked = false;
        }

        public override void SetCount(int count)
        {
            _count = count;
            _countText.text = $"x{count}";
        }

        public override int GetCount()
        {
            return _count;
        }
    }
}