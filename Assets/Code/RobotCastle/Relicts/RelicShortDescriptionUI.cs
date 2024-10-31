using DG.Tweening;
using RobotCastle.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Relics
{
    public class RelicShortDescriptionUI : DescriptionUI
    {

        public void Setup(RelicData relicData)
        {
            _textName.text = relicData.viewName;
            _textDescription.text = relicData.description;
            _icon.sprite = Resources.Load<Sprite>(relicData.icon);
            _textTier.text = $"Tier {relicData.core.tier + 1}";
            var mod = RelicsManager.GetStatModFromRelic(relicData) ;
            if (mod != null)
                _stats.ShowStatMod(mod);
        }

        public void Show(RelicItemUI item)
        {
            Setup(item.relicData);
            gameObject.SetActive(true);
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(1f, .231f).OnComplete(() =>
            {
                gameObject.SetActive(true);
            });
        }

        public override void Show(GameObject source)
        {
            var item = source.GetComponent<RelicItemUI>();
            if (item == null || item.relicData == null)
            {
                Debug.LogError("Null or empty");
                return;
            }
            Show(item);
        }

        public override void Hide()
        {
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0f, .231f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private TextMeshProUGUI _textTier;
        [SerializeField] private Image _icon;
        [SerializeField] private RelicStatsUI _stats;
        [SerializeField] private CanvasGroup _canvasGroup;

    }
}