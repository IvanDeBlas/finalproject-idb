namespace WePlayRises.BuildingBlocks.Kernel.Model
{
    public class BaseEntity<TEntity, TKey>
         where TEntity : class, IEntity<TKey>
    {
        public virtual TEntity Entity { get; set; }
        public virtual List<string> Includes { get; set; } = null;
    }
}