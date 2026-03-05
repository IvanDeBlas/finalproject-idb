namespace WePlayRises.BuildingBlocks.Kernel.Model
{

    /// <summary>
	/// Wrapper for paginated results with metadata
	/// </summary>
	/// <typeparam name="T">The type of items in the list</typeparam>
	public class PagedList<T> where T : class
    {
        public PagedList()
        {
            Data = new List<T>();
        }

        public PagedList(List<T> items, int totalCount, int currentPage, int pageSize)
        {
            Data = items;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// Items in the current page
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// Total count of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Size of each page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indicates if there is a previous page
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Indicates if there is a next page
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;
    }
}
