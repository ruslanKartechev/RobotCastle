using TMPro;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    public class AltarModifierUI : MonoBehaviour
    {
        public static readonly Color InactiveColorVal = new (0.133f, 0.133f, 0.133f, 1);

        [SerializeField] private TextMeshProUGUI _descriptionShort;
        [SerializeField] private TextMeshProUGUI _descriptionLongs;
        [SerializeField] private bool _longMode;
        [SerializeField] private FadeEffect _fadeEffect;
        private AltarMP _mp;
        private bool _didSubEvents;
        
        public void Init(AltarMP mp)
        {
            _descriptionShort.text = mp.GetShortDescription();
            if (_longMode)
                _descriptionLongs.text = mp.GetDetailedDescription();
            if (_mp != mp && !_didSubEvents)
            {
                _didSubEvents = true;
                mp.OnLocked += OnLocked;
                mp.OnUnlocked += Unlocked;
                mp.OnTierUpdated += TierUpdated;
            }
            _mp = mp;
            SetColor(mp.IsActive);
            SetDescription();
        }

        private void OnDisable()
        {
            if (_mp != null)
            {
                _didSubEvents = false;
                _mp.OnLocked -= OnLocked;
                _mp.OnUnlocked -= Unlocked;
                _mp.OnTierUpdated -= TierUpdated;
                _mp = null;
            }
        }

        private void TierUpdated(int from, int to)
        {
            _descriptionShort.text = _mp.GetShortDescription();
        }
        
        private void Unlocked()
        {
            SetColor(true);
            _fadeEffect.Play();
        }

        private void OnLocked()
        {
            SetColor(false);
        }

        private void SetColor(bool active)
        {
            if (active)
            {
                _descriptionShort.color = Color.white;
                if (_longMode)
                    _descriptionLongs.color = Color.white;
            }
            else
            {
                _descriptionShort.color = InactiveColorVal;
                if (_longMode)
                    _descriptionLongs.color = InactiveColorVal;
            }
        }
        
        private void SetDescription()
        {
            _descriptionShort.text = _mp.GetShortDescription();
            if (_longMode)
                _descriptionLongs.text = _mp.GetDetailedDescription();
        }
        
    }
}