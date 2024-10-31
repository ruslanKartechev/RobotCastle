using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Relics
{
    public class RelicStatsUI : MonoBehaviour
    {
        public void On() => gameObject.SetActive(true);
 
        public void Off() => gameObject.SetActive(false);

        public void ShowStatMod(RelicStatModifier modifier)
        {
            _atkView.Show(modifier.GetAtkMod());
            _atkSpeedView.Show(modifier.GetAtkSpeedMod());
            _defView.Show(modifier.GetDEf());
            _healthView.Show(modifier.GetHealth());
        }

        [SerializeField] private StatView _atkView;
        [SerializeField] private StatView _atkSpeedView;
        [SerializeField] private StatView _defView;
        [SerializeField] private StatView _healthView;
        
        
        [System.Serializable]
        public class StatView
        {
            public float maxValue;
            public List<Image> images;
            public Color colorActive;
            public Color colorPassive;
            public TextMeshProUGUI text;
            public bool textIsPercent = true;
            
            public void Show(float value)
            {
                if (text != null)
                {
                    if (textIsPercent)
                        text.text = $"{Mathf.RoundToInt(value * 100)}%";
                    else
                        text.text = $"{Mathf.RoundToInt(value)}";
                }
                int count;
                if (value >= maxValue)
                    count = images.Count;
                else
                    count = Mathf.CeilToInt( (value / maxValue) * images.Count );
                for (var i = 0; i < count; i++)
                    images[i].color = colorActive;
                for (var i = count; i < images.Count; i++)
                    images[i].color = colorPassive;

            }
        }
    }
}