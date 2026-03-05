namespace WePlayRises.BuildingBlocks.Kernel.Dtos
{
    /// <summary>
    /// Generic wrapper DTO for list responses.
    /// Allows extensibility (pagination, metadata, etc.) without breaking changes.
    /// </summary>
    /// <typeparam name="T">The type of items in the list</typeparam>
    public class ListResponse<T>
    {
        /// <summary>
        /// The list of items.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Total count of items (useful for future pagination).
        /// </summary>
        public int Count => Items.Count;
    }
}
