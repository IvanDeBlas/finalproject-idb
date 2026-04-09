using AutoMapper;
using FluentValidation;
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Model;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetAllPaged
{
    /// <summary>
    /// Base handler for paginated queries that returns PagedList with metadata
    /// </summary>
    /// <typeparam name="TEntity">The entity type from the domain</typeparam>
    /// <typeparam name="TDto">The DTO type to return</typeparam>
    public class GetAllPagedQueryHandlerBase<TEntity, TDto>
        : IRequestHandler<PagedQueryBase<TDto>, ServiceResponse<PagedList<TDto>>>
        where TEntity : class
        where TDto : class
    {
        private readonly IGetAll<TEntity> _service;
        protected readonly IMapper _mapper;
        private readonly IValidator<PagedQueryBase<TDto>> _validator;

        public GetAllPagedQueryHandlerBase(
            IGetAll<TEntity> service,
            IMapper mapper,
            IValidator<PagedQueryBase<TDto>> validator)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<ServiceResponse<PagedList<TDto>>> Handle(
            PagedQueryBase<TDto> request,
            CancellationToken cancellationToken)
        {
            // Validate pagination parameters
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PagedList<TDto>>
                {
                    Messages = validationResult.Errors
                        .Select(e => new ServiceResponseMessage
                        {
                            Message = e.ErrorMessage,
                            ErrorCode = e.ErrorCode
                        })
                        .ToList()
                };
            }

            // Get all entities
            var allEntities = _service.GetAll().ToList();
            var totalCount = allEntities.Count;

            // Apply pagination
            var pagedEntities = allEntities
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var dtos = _mapper.Map<List<TEntity>, List<TDto>>(pagedEntities);

            // Create paged result
            var pagedList = new PagedList<TDto>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return new ServiceResponse<PagedList<TDto>>
            {
                Data = pagedList
            };
        }
    }
}
