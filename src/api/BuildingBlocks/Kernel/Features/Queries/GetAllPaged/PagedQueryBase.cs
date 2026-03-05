using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Model;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetAllPaged
{
    /// <summary>
    /// Base class for paginated queries
    /// </summary>
    /// <typeparam name="TDto">The DTO type to return</typeparam>
    public class PagedQueryBase<TDto> : IRequest<ServiceResponse<PagedList<TDto>>>
        where TDto : class
    {
        /// <summary>
        /// Current page number (1-based). Default is 1.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page. Default is 10.
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
