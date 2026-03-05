namespace WePlayRises.BuildingBlocks.Kernel.Services
{
    public interface IGetAll<TEntity>
    {
        List<TEntity> GetAll();
    }
}
