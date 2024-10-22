using SleepDev;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class MergeInfoUI : MonoBehaviour, IMergeInfoUI
    {
        [SerializeField] private TextMeshProUGUI _textTroopSize;
        [SerializeField] private FadePopAnimator _notEnoughTroopSizeBlock;
        [SerializeField] private FadePopAnimator _notEnoughMoneyBlock;
        [SerializeField] private FadePopAnimator _notEnoughSpaceBlock;
        [SerializeField] private FadePopAnimator _noHeroesOnGrid;

        public void ShowNotEnoughTroopSize(int count, int max)
        {
            _textTroopSize.text = $"Not enough troop size {count}/{max}";
            _notEnoughTroopSizeBlock.Animate();
        }

        public void ShowNotEnoughMoney()
        {
            _notEnoughMoneyBlock.Animate();
        }

        public void ShowNotEnoughSpace()
        {
            _notEnoughSpaceBlock.Animate();
        }

        public void ShowNoHeroesOnGrid()
        {
            _noHeroesOnGrid.Animate();
        }
        
        public void ShowIdle()
        {
            gameObject.SetActive(true);
            _notEnoughTroopSizeBlock.gameObject.SetActive(false);
            _notEnoughMoneyBlock.gameObject.SetActive(false);
            _notEnoughSpaceBlock.gameObject.SetActive(false);
            _noHeroesOnGrid.gameObject.SetActive(false);
        }

        public void On() => gameObject.SetActive(true);
        public void Off() => gameObject.SetActive(false);
    }
}