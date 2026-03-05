using AutoMapper;
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetById
{
    public class GetByIdQueryHandlerBase<TEntity, TDto, TKey> : IRequestHandler<GetByIdQueryBase<TDto, TKey>, TDto>
        where TEntity : class // TEntity debe ser un tipo de referencia
        where TDto : class // TDto también debe ser un tipo de referencia
        where TKey : IEquatable<TKey> // TKey debe implementar IEquatable<TKey>
    {
        private readonly IGetById<TEntity, TKey> _service;
        private readonly IMapper _mapper;

        public GetByIdQueryHandlerBase(IGetById<TEntity, TKey> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public virtual async Task<TDto> Handle(GetByIdQueryBase<TDto, TKey> request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            //// Llama al servicio para obtener la entidad
            //var entity = await _service.GetByIdAsync(request.Id, cancellationToken);
            var entity = _service.GetById(request.Id);
            if (entity == null)
            {
                return default!;
            }

            // Mapea la entidad al DTO
            return _mapper.Map<TDto>(entity);
        }
    }
}
