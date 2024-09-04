using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SleepDev
{
    public class StarsLevelView : MonoBehaviour
    {
        [SerializeField] private StarsViewDataBase _dataBase;
        [SerializeField] private List<Image> _images;
        [SerializeField] private HorizontalFitter _horizontalFitter;
        [SerializeField] private RectTransform _animatable;
        
        public void SetLevel(int levelIndex)
        {
            for (var imageInd = _dataBase.levelsThresholds.Count - 1; imageInd >= 0; imageInd--)
            {
                if (levelIndex >= _dataBase.levelsThresholds[imageInd])
                {
                    var starsCount = levelIndex - _dataBase.levelsThresholds[imageInd] + 1;
                    SetStarsCount(starsCount, imageInd);
                    break;
                }
            }
        }

        public void SetStarsCount(int count, int iconIndex)
        {
            var icon = _dataBase.images[iconIndex];
            for (var i = 0; i < count; i++)
            {
                _images[i].sprite = icon;
                _images[i].gameObject.SetActive(true);
            }
            for (var i = count; i < _images.Count; i++)
            {
                _images[i].gameObject.SetActive(false);
            }
            _horizontalFitter.FitActive();
        }

        public void AnimateUpdated()
        {
            _animatable.localScale = new Vector3(.9f, .5f, 1f);
            _animatable.DOScale(Vector3.one, .15f).SetEase(Ease.InBack);
        }
        
        
        
        #if UNITY_EDITOR
        [ContextMenu("TestSet0")]
        public void TestSet0() => SetLevel(0);
        
        [ContextMenu("TestSet1")]
        public void TestSet1() => SetLevel(1);

        [ContextMenu("TestSet2")]
        public void TestSet2() => SetLevel(2);

        [ContextMenu("TestSet3")]
        public void TestSet3() => SetLevel(3);
        
        [ContextMenu("TestSet4")]
        public void TestSet4() => SetLevel(4);

        [ContextMenu("TestSet5")]
        public void TestSet5() => SetLevel(5);
        
        [ContextMenu("TestSet8")]
        public void TestSet8() => SetLevel(8);

#endif
    }
    
}