using AutoMapper;
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Model;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.Filter
{
    public class FilterQueryHandlerBase<TEntity, TDto> : IRequestHandler<FilterQueryBase<TDto>, PagedList<TDto>>
           where TDto : class
           where TEntity : class
    {
        private readonly IGetEntitiesAsync<TEntity> _service;
        protected readonly IMapper _mapper;

        public FilterQueryHandlerBase(IGetEntitiesAsync<TEntity> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<TDto>> Handle(FilterQueryBase<TDto> request, CancellationToken cancellationToken)
        {
            var result = await _service.GetEntitiesAsync(request.SearchFilter, cancellationToken);

            return _mapper.Map<PagedList<TEntity>, PagedList<TDto>>(result);
        }
    }
}