namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IItemExists<TKey>
    {
        bool ItemExists(TKey id);
        (bool exists, TKey? id) ItemExistsWithName(string name);
    }
}
