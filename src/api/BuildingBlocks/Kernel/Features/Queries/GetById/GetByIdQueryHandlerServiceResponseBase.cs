using AutoMapper;
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetById
{
    public class GetByIdQueryHandlerServiceResponseBase<TEntity, TDto, TKey> : IRequestHandler<GetByIdQueryServiceResponseBase<TDto, TKey>, ServiceResponse<TDto>>
        where TEntity : class // TEntity debe ser un tipo de referencia
        where TDto : class // TDto también debe ser un tipo de referencia
        where TKey : IEquatable<TKey> // TKey debe implementar IEquatable<TKey>
    {
        private readonly IGetById<TEntity, TKey> _service;
        private readonly IMapper _mapper;

        public GetByIdQueryHandlerServiceResponseBase(IGetById<TEntity, TKey> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public virtual async Task<ServiceResponse<TDto>> Handle(GetByIdQueryServiceResponseBase<TDto, TKey> request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            //// Llama al servicio para obtener la entidad
            //var entity = await _service.GetByIdAsync(request.Id, cancellationToken);
            var entity = _service.GetById(request.Id);
            if (entity == null)
            {
                return default!;
            }

            var result = _mapper.Map<TDto>(entity);

            await Task.CompletedTask;

            // Mapea la entidad al DTO
            return new ServiceResponse<TDto>() { Data = result };
        }
    }
}
