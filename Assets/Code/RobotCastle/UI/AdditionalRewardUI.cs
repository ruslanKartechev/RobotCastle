using RobotCastle.Data;
using RobotCastle.InvasionMode;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class AdditionalRewardUI : MonoBehaviour
    {
        public ChapterSelectionData SelectionData { get; set; }

        [SerializeField] private GameObject _go;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI _txtAdditionalEnergyCost;
        [SerializeField] private TextMeshProUGUI _txtRewardMultiplier;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI _txtEnergyCostBtn;
        [SerializeField] private TextMeshProUGUI _txtEnergyStat;
        [Space(10)] 
        [SerializeField] private MyButton _btnNext;
        [SerializeField] private MyButton _btnPrev;
        

        public void On()
        {
            _go.SetActive(true);
        }

        public void Off()
        {
            _go.SetActive(false);
        }

        public void Init()
        {
            if (SelectionData == null)
            {
                CLog.LogError("Selection data is null!!");
                return;
            }
            _btnPrev.AddMainCallback(PrevMultiplierTier);
            _btnNext.AddMainCallback(NextMultiplierTier);
        }

        private void PrevMultiplierTier()
        {
            var tier = SelectionData.multiplierTier;
            if (tier <= ChapterSelectionData.MinMultiplierTier)
            {
                SelectionData.multiplierTier = ChapterSelectionData.MinMultiplierTier;
                return;
            }
            tier--;
            SelectionData.multiplierTier = tier;
            UpdateDataView();
        }
        
        private void NextMultiplierTier()
        {
            var tier = SelectionData.multiplierTier;
            if (tier >= ChapterSelectionData.MaxMultiplierTier)
            {
                SelectionData.multiplierTier = ChapterSelectionData.MaxMultiplierTier;
                return;
            }
            tier++;
            SelectionData.multiplierTier = tier;
            UpdateDataView();
        }

        public void UpdateDataView()
        {
            var data = DataHelpers.GetPlayerData();
            _txtEnergyStat.text = $"{data.playerEnergy}/{data.playerEnergyMax}";
            _txtRewardMultiplier.text = $"Reward x{SelectionData.multiplierTier}";
            var cost = SelectionData.totalEnergyCost = SelectionData.multiplierTier * ChapterSelectionData.BasicEnergyCost;

            _txtAdditionalEnergyCost.text = _txtEnergyCostBtn.text = $"-{cost}";
            
            _btnPrev.SetInteractable(SelectionData.multiplierTier > 1);
            _btnNext.SetInteractable(SelectionData.multiplierTier < 3);

        }
    }
}