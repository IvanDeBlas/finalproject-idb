using System.Linq.Expressions;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions.LinQ
{
    public static class QueryableExtensions
    {
        public static bool TryGetValue<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate, out IQueryable<T>? result)
        {
            if (queryable.Any(predicate))
            {
                result = queryable.Where(predicate);
                return true;
            }

            result = null;
            return false;
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? queryable.Where(predicate)
                : queryable;
        }


        public static IQueryable<T> TakeIf<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> orderBy, bool condition, int limit, bool orderByDescending = true)
        {
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return condition
                ? query.Take(limit)
                : query;
        }


        public static IQueryable<T> PageBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> orderBy, int page, int pageSize, bool orderByDescending = true)
        {
            const int defaultPageNumber = 1;

            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (page <= 0)
                page = defaultPageNumber;

            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

    }
}