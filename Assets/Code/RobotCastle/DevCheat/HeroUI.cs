using RobotCastle.Core;
using RobotCastle.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.DevCheat
{
    public class HeroUI : MonoBehaviour
    {
        public const int MaxLvl = 19;
        public const int MinLvl = 0;
        
        public void Init(HeroSave save, string name, Sprite icon)
        {
            _save = save;
            _textName.text = name;
            _iconImage.sprite = icon;
            _unlockedBtn.SetState(save.isUnlocked);
            SetLevel();
            if (!_didSub)
            {
                _didSub = true;
                _addLevelBtn.AddMainCallback(AddLevel);
                _removeLevelBtn.AddMainCallback(RemoveLvl);
                _unlockedBtn.AddMainCallback(ChangeUnlockedState);
            }
        }

        public void Unlock()
        {
            if (_save != null)
            {
                _save.isUnlocked = true;
                _unlockedBtn.SetState(true);
            }
        }
        
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private MyButton _addLevelBtn;
        [SerializeField] private MyButton _removeLevelBtn;
        [SerializeField] private ButtonWithCheck _unlockedBtn;
        private HeroSave _save;
        private bool _didSub;

        private void ChangeUnlockedState()
        {
            var ss = !_save.isUnlocked;
            _save.isUnlocked = ss;
            _unlockedBtn.SetState(ss);
        }

        private void AddLevel()
        {
            var lvl = _save.level + 1;
            if (lvl > MaxLvl)
                return;
            _save.level = lvl;
            SetLevel();
        }

        private void SetLevel()
        {
            _textLevel.text = $"Level {_save.level + 1}";
        }

        private void RemoveLvl()
        {
            var lvl = _save.level - 1;
            if (lvl < MinLvl)
                return;
            _save.level = lvl;
            SetLevel();
        }
        
        
    }
}