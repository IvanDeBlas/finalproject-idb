using WePlayRises.BuildingBlocks.Kernel.Model;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    public static class ListExtensions
    {
        public static PagedList<TEntity> ToPagedList<TEntity>(this IQueryable<TEntity> query, int page, int pageSize) where TEntity : class
        {
            var totalCount = query.Count();
            var entities = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<TEntity>()
            {
                Data = entities,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
