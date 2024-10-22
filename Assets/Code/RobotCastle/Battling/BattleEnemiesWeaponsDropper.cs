using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleEnemiesWeaponsDropper : IBattleHeroKilledListener
    {
        public const string animationPrefab = "prefabs/enemies_drop_anim";
        

        public void Reset()
        {
            CLog.Log($"[{nameof(BattleEnemiesWeaponsDropper)}] Reset");
            for (var i = 0; i < _animations.Count; i++)
                Object.Destroy(_animations[i].gameObject);
            _animations.Clear();
            _allCollected.Clear();
        }

        public void RevealAll()
        {
            CLog.Log($"[{nameof(BattleEnemiesWeaponsDropper)}] RevealAll");
            foreach (var view in _allCollected)
                view.Transform.gameObject.SetActive(true);
        }
        
        public void OnKilled(IHeroController hero)
        {
            if (hero.Components.weaponsContainer.ItemsCount > 0)
            {
               CLog.Log($"[{nameof(BattleEnemiesWeaponsDropper)}] OnKilled with weapons");
                var items = hero.Components.weaponsContainer.Items;
                var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
                var merge = ServiceLocator.Get<MergeManager>();
                var weaponsDropped = new List<IItemView>(3);
                for (var i = 0; i < items.Count; i++)
                {
                    var it = items[i];
                    var args = new SpawnMergeItemArgs(it.core);
                    var didSpawn = factory.SpawnHeroOrItem(args, merge.GridView, merge.SectionsController, out var view);
                    if (didSpawn)
                    {
                        weaponsDropped.Add(view);
                        _allCollected.Add(view);
                        view.Hide();
                    }
                }
                var animation = Object.Instantiate(Resources.Load<EnemiesDropAnimation>(animationPrefab));
                animation.Show(hero.Components.transform.position, weaponsDropped);
                _animations.Add(animation);
            }
        }   
        
        private List<EnemiesDropAnimation> _animations = new (5);
        private List<IItemView> _allCollected = new (10);

    }
}