using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeItemsFactory : MonoBehaviour, IMergeItemsFactory
    {
        public static void Rename(GameObject instance)
        {
            instance.gameObject.name = instance.gameObject.name.Replace(ReplaceString, $"_{(int)(UnityEngine.Random.Range(0, 1f) * NameMult)}");
        }
        
        private const int NameMult = 10000;
        private const string ReplaceString = "(Clone)";
        
        public IItemView SpawnItemOnCell(ICellView pivotCell, ItemData itemData)
        {
            var db = ServiceLocator.Get<ViewDataBaseContainer>();
            GameObject prefab = null;
            GameObject instance = null;
            switch (itemData.core.type)
            {
                case MergeConstants.TypeWeapons:
                    prefab = db.viewDb.GetMergePrefabAtLevel(itemData.core.id, itemData.core.level);
                    break;
                case MergeConstants.TypeHeroes:
                    prefab = db.viewDb.GetMergePrefab(itemData.core.id);
                    break;
                default:
                    CLog.LogRed($"[GridItemsSpawner] {itemData.core.type} Unknown type");
                    break;
            }
            instance = SleepDev.MiscUtils.Spawn(prefab, transform);
            Rename(instance);
            var itemView = instance.GetComponent<IItemView>();
            itemView.InitView(itemData);
            MergeFunctions.PutItemToCell(itemView, pivotCell);
            // extend for case of multi cell items!
            return itemView;
        }

   
        public List<IItemView> SpawnItems(List<CoreItemData> items)
        {
            var db = ServiceLocator.Get<ViewDataBaseContainer>();
            var result = new List<IItemView>(items.Count);
            foreach (var itemData in items)
            {
                GameObject prefab = null;
                GameObject instance = null;
                switch (itemData.type)
                {
                    case MergeConstants.TypeWeapons:
                        prefab = db.viewDb.GetMergePrefabAtLevel(itemData.id, itemData.level);
                        break;
                    case MergeConstants.TypeHeroes:
                        prefab = db.viewDb.GetMergePrefab(itemData.id);
                        break;
                    default:
                        CLog.LogRed($"[GridItemsSpawner] {itemData.type} Unknown type");
                        break;
                }
                instance = SleepDev.MiscUtils.Spawn(prefab, transform);
                var view = instance.GetComponent<IItemView>();
                if (view == null)
                {
                    CLog.LogError($"IItemView is null on: {instance.gameObject.name}");
                }
                else
                    result.Add(view);
            }
            return result;
        }
        
    }
}