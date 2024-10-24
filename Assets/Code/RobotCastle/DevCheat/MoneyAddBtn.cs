using RobotCastle.UI;
using SleepDev.Data;
using TMPro;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public class MoneyAddBtn : MonoBehaviour
    {
        public int addedAmount = 1000;
      

        public ReactiveInt source
        {
            get => _source;
            set
            {
                var old = _source;
                _source = value;
                if(_source != null)
                {
                    if (old != null && old != value)
                    {
                        old.OnSet -= OnSet;
                    }
                    _text.text = _source.Val.ToString();
                    _source.OnSet += OnSet;
                }
                else
                {
                    
                }
            }
        }

        
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private MyButton _addBtn;
        private ReactiveInt _source;

        private void OnEnable()
        {
            _addBtn.AddMainCallback(AddMoney);
        }

        private void OnDisable()
        {
            if (_source != null)
            {
                _source.OnSet -= OnSet;
            }
        }

        private void AddMoney()
        {
            if(_source != null)
                _source.AddValue(addedAmount);
        }
        
        private void OnSet(int newval, int oldval)
        {
            _text.text = newval.ToString();
        }
    }
}