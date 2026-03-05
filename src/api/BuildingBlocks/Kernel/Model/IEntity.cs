namespace WePlayRises.BuildingBlocks.Kernel.Model
{
    public interface IEntity<TKey> : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
