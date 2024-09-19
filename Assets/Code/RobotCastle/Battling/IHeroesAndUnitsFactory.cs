using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public interface IHeroesAndUnitsFactory
    {
        bool SpawnHeroOrItem(SpawnMergeItemArgs args,
            IGridView grid,
            IGridSectionsController sectionsController,
            out IItemView spawnedItem);

        void SpawnHeroOrItem(SpawnMergeItemArgs args, 
            ICellView cellView, 
            out IItemView spawnedItem);
    }
}