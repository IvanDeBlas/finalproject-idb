using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetMisCampaniasQuery : IRequest<ServiceResponse<IEnumerable<CampaniaListDto>>>
{
    public Guid ArtistaId { get; set; }
    public int? EstadoCampaniaId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetMisCampaniasQueryHandler : IRequestHandler<GetMisCampaniasQuery, ServiceResponse<IEnumerable<CampaniaListDto>>>
{
    private readonly ICampaniaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMisCampaniasQueryHandler> _logger;

    public GetMisCampaniasQueryHandler(
        ICampaniaService service,
        IMapper mapper,
        ILogger<GetMisCampaniasQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<IEnumerable<CampaniaListDto>>> Handle(
        GetMisCampaniasQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var artistaId = new ArtistaId(request.ArtistaId);
            var entities = await _service.GetByArtistaIdAsync(artistaId, cancellationToken);

            var filtered = entities.Where(e => !e.Borrado);

            // Filter by state (optional, NO default applied)
            if (request.EstadoCampaniaId.HasValue)
            {
                filtered = filtered.Where(c => c.EstadoCampaniaId == request.EstadoCampaniaId.Value);
            }

            // Pagination
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = Math.Min(request.PageSize ?? 10, 50);

            var paginatedEntities = filtered
                .OrderByDescending(c => c.FechaCreacion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = _mapper.Map<IEnumerable<CampaniaListDto>>(paginatedEntities);

            return new ServiceResponse<IEnumerable<CampaniaListDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campanias encontradas",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Campanias for Artista {ArtistaId}", request.ArtistaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<IEnumerable<CampaniaListDto>>(
                "Error inesperado al listar campanias",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
