using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.Saving;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class PlayersPartyGateDisplay : MonoBehaviour
    {
        [SerializeField] private List<MergeCellView> _cells;
        private readonly List<GameObject> _instances = new(6);
        private bool _didInit;

        private void Init()
        {
            for (var i = 0; i < HeroesConstants.PartySize; i++)
            {
                _instances.Add(null);
                _cells[i].cell = new Cell(i, 0);
            }
        }

        public void SetFromSave()
        {
            if (!_didInit)
            {
                _didInit = true;
                Init();
            }
            var save = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().party;
            var pool = ServiceLocator.Get<BarracksHeroesPool>();
            for (var i = 0; i < HeroesConstants.PartySize; i++)
            {
                if (string.IsNullOrEmpty(save.heroesIds[i]))
                {
                    if (i < _instances.Count && _instances[i] != null)
                        _instances[i].SetActive(false);
                    continue;
                }
                var hero = pool.GetHero(save.heroesIds[i]);
                MergeFunctions.PutItemToCell(hero, _cells[i]);
                hero.Transform.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            
        }
        
    }
}