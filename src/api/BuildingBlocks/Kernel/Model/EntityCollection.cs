namespace WePlayRises.BuildingBlocks.Kernel.Model
{
    public class EntityCollection<TEntity, T> : IEntity<T>
    {
        public T Id { get; set; }

        public IEnumerable<TEntity> Collection { get; set; }

        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(Id, other);
        }
    }
}
