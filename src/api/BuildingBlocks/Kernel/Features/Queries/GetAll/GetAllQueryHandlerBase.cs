using AutoMapper;
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Dtos;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetAll
{
    public class GetAllQueryHandlerBase<TEntity, TDto> : IRequestHandler<GetAllQueryBase<TDto>, ServiceResponse<ListResponse<TDto>>>
    {
        private readonly IGetAll<TEntity> _service;
        protected readonly IMapper _mapper;

        public GetAllQueryHandlerBase(IGetAll<TEntity> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceResponse<ListResponse<TDto>>> Handle(GetAllQueryBase<TDto> request, CancellationToken cancellationToken)
        {
            var result = _service.GetAll();

            return new ServiceResponse<ListResponse<TDto>>
            {
                Data = new ListResponse<TDto>
                {
                    Items = _mapper.Map<List<TEntity>, List<TDto>>(result.ToList())
                }
            };
        }
    }
}
