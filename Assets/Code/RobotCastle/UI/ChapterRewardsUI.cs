using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Relics;
using SleepDev;
using UnityEngine;

namespace RobotCastle.UI
{
    public class ChapterRewardsUI : MonoBehaviour
    {
        [SerializeField] private Sprite _goldSprite;
        [SerializeField] private List<ChapterRewardItemUI> _rewardItems;
        [SerializeField] private float _animationTime = .22f;

        public void ModifyGold(int gold)
        {
            _rewardItems[0].SetCount(gold);
        }
        
        public void SetRewards(int gold, List<CoreItemData> rewards)
        {
            var uiInd = 0;
            _rewardItems[uiInd].SetIconAndCount(_goldSprite, gold);
            _rewardItems[uiInd].gameObject.SetActive(true);
            if (rewards == null || rewards.Count == 0)
            {
                for(var i = 1; i < _rewardItems.Count; i++)
                    _rewardItems[i].Off();
                return;
            }
            uiInd++;
            for (var i = 0; i < rewards.Count && i < _rewardItems.Count; i++)
            {
                var rew = rewards[i];
                // CLog.LogRed($"Reward: {rew.id} {rew.level}");
                Sprite icon = null;
                switch (rew.type)
                {
                    case "relic":
                        icon = RelicsManager.GetIconForRelic(rew.id);
                        break;
                    case "item":
                        var db = ServiceLocator.Get<ViewDataBase>();
                        icon = db.GetGeneralItemSprite(rew.id);
                        break;
                }
                _rewardItems[uiInd].SetIconAndCount(icon, rew.level);
                _rewardItems[uiInd].gameObject.SetActive(true);
                uiInd++;
            }
            for(var i = uiInd; i < _rewardItems.Count; i++)
                _rewardItems[i].gameObject.SetActive(false);
            var scale1 = new Vector3(.9f, .5f, 1);
            var scale2 = new Vector3(1, 1, 1);
            for (var i = 0; i < uiInd; i++)
            {
                _rewardItems[i].transform.localScale = scale1;
                _rewardItems[i].transform.DOScale(scale2, _animationTime);
            }
        }
        
    }
}