using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class HeroesPartyUI : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private List<Image> _heroIcons;
        [SerializeField] private TextMeshProUGUI _textPlayerPower;

        public void On()
        {
            _canvas.enabled = true;
            gameObject.SetActive(true);
        }

        public void Off()
        {
            _canvas.enabled = false;
        }
        
        public void SetupHeroes()
        {
            var party = DataHelpers.GetPlayerParty();
            var count = _heroIcons.Count;
            var db = ServiceLocator.Get<HeroesDatabase>();
            for (var i = 0; i < count; i++)
            {
                var id = party.heroesIds[i];
                _heroIcons[i].sprite = ViewDataBase.GetHeroSprite(db.GetHeroViewInfo(id).iconId);
            }
            var power = HeroesPowerCalculator.CalculateTotalPlayerPower(party.heroesIds);
            _textPlayerPower.text = power.ToString();

        }
    }
}