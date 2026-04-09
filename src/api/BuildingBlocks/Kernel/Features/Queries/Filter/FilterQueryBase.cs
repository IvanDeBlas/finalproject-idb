using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Model;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.Filter
{
    public class FilterQueryBase<TDto> : IRequest<PagedList<TDto>>
         where TDto : class
    {
        public SearchFilter SearchFilter { get; set; }
    }
}
