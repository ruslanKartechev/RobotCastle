using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class InvasionLevelsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelNumText;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private RectTransform _flag;
        [SerializeField] private List<Image> _images;
        [SerializeField] private List<Sprite> _iconSpritesByType;

        public string LevelName { get; set; }

        public void SetLevel(int levelIndex, List<RoundData> roundData,  bool animated)
        {
            SetRewardForWave(roundData[levelIndex].reward);
            _levelNumText.text = $"{LevelName} | Wave {levelIndex + 1}";
            var nextIconInd = levelIndex % _images.Count;
            if (nextIconInd == 0)
            {
                for (var i = 0; i < _images.Count; i++)
                {
                    var lvlInd = levelIndex + i;
                    _images[i].sprite = _iconSpritesByType[(int)roundData[lvlInd].roundType];
                }
            }

            var endPos = _images[nextIconInd].rectTransform.anchoredPosition;
            if (animated)
            {
                const float height = 90f;
                const float time = .2f;
                var pMid = Vector2.Lerp(_flag.anchoredPosition, endPos, .5f);
                pMid.y += height;
                _flag.DOAnchorPos(pMid, time).OnComplete(() =>
                {
                    _flag.DOAnchorPos(endPos, time);
                });
            }
            else
            {
                _flag.anchoredPosition = endPos;
            }
        }

        public void SetRewardForWave(int reward)
        {
            _rewardText.text = reward.ToString();
        }
        
    }
}