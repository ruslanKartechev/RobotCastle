namespace RobotCastle.Merging
{
    public interface IMergeModifier
    {
        void OnMergedOneIntoAnother(IItemView itemHidden, IItemView itemMergedInto);

        void OnNewItemSpawnDuringMerge(IItemView newItem, IItemView item1, IItemView item2);
    }
}