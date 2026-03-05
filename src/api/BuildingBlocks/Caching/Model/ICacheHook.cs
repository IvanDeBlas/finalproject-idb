namespace WePlayRises.BuildingBlocks.Caching.Model
{
    /// <summary>
    /// Interface to hook the local in-memory cache.  This interface will allow
    /// for clients to perform actions on the items being stored in the in-memory
    /// cache.
    /// </summary>
    public interface ICacheHook
    {
        /// <summary>
        /// Prepare the object for storing in the cache.
        /// </summary>
        T Put<T>(T t);

        /// <summary>
        /// Derive the object from the cached object.
        /// </summary>
        T Get<T>(T t);
    }
}
