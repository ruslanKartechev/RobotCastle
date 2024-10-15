using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Battling.Altars
{
    public class FadeEffect : MonoBehaviour 
    {
        public const float FadeTime = .19f;

        [SerializeField] private Image _fadeImage;

        public void Play()
        {
            _fadeImage.enabled = true;
            _fadeImage.DOKill();
            _fadeImage.DOFade(1f, FadeTime).OnComplete(() =>
            {
                _fadeImage.DOFade(0f, FadeTime).OnComplete(() =>
                {
                    _fadeImage.enabled = false;
                });
            });
        }
    }
}