using System.Linq.Expressions;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions.LinQ
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }
        public static IEnumerable<T> TakeIf<T, TKey>(this IEnumerable<T> query, Func<T, TKey> orderBy, bool condition, int limit, bool orderByDescending = true)
        {
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return condition
                ? query.Take(limit)
                : query;
        }
        public static IEnumerable<T> MoveToEnd<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
        {
            List<T> pending = new();
            foreach (var item in seq)
            {
                if (predicate(item)) pending.Add(item);
                else yield return item;
            }

            foreach (var item in pending) yield return item;
        }
    }
}