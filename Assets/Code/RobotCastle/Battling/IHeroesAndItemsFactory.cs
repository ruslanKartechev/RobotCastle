using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IHeroesAndItemsFactory
    {
        bool SpawnHeroOrItem(SpawnArgs args,
            IGridView grid,
            IGridSectionsController sectionsController,
            out IItemView spawnedItem);

        void SpawnOnCell(SpawnArgs args, 
            ICellView cellView, 
            out IItemView spawnedItem);

        /// <summary>
        /// Instantiates prefab and initializes hero. Does not do anything merge-related
        /// </summary>
        IHeroController SpawnHero(SpawnArgs args, Vector3 position, Quaternion rotation);
    }
}