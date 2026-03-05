using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetMisPropuestasQuery : IRequest<ServiceResponse<PaginatedResponse<MiPropuestaListDto>>>
{
    public string UserId { get; set; } = null!;
    public int? EstadoPropuestaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetMisPropuestasQueryHandler : IRequestHandler<GetMisPropuestasQuery, ServiceResponse<PaginatedResponse<MiPropuestaListDto>>>
{
    private readonly IPropuestaCrowdsourcingService _service;
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMisPropuestasQueryHandler> _logger;

    private static readonly Dictionary<int, string> EstadoPropuestaNames = new()
    {
        { EstadoPropuestaConstants.Pendiente, "Pendiente" },
        { EstadoPropuestaConstants.Aceptada, "Aceptada" },
        { EstadoPropuestaConstants.Rechazada, "Rechazada" },
        { EstadoPropuestaConstants.Retirada, "Retirada" }
    };

    public GetMisPropuestasQueryHandler(
        IPropuestaCrowdsourcingService service,
        IArtistaService artistaService,
        IMapper mapper,
        ILogger<GetMisPropuestasQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PaginatedResponse<MiPropuestaListDto>>> Handle(
        GetMisPropuestasQuery request,
        CancellationToken ct)
    {
        try
        {
            var (items, totalCount) = await _service.GetByUserIdPaginatedAsync(
                request.UserId,
                request.EstadoPropuestaId,
                request.Page,
                Math.Min(request.PageSize, 50),
                ct);

            var dtos = _mapper.Map<List<MiPropuestaListDto>>(items);

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var dto = dtos[i];

                dto.NecesidadTitulo = item.Necesidad?.Titulo ?? string.Empty;

                if (item.Necesidad != null)
                {
                    var itemArtista = await _artistaService.GetByIdAsync(item.Necesidad.ArtistaId, ct);
                    dto.ArtistaNombre = itemArtista?.NombreArtistico ?? string.Empty;
                }
                else
                {
                    dto.ArtistaNombre = string.Empty;
                }

                dto.EstadoPropuestaNombre = EstadoPropuestaNames.GetValueOrDefault(
                    item.EstadoPropuestaId, "Desconocido");

                dto.AcuerdoId = item.Acuerdos?.FirstOrDefault()?.Id.Value;
            }

            var paginatedResponse = new PaginatedResponse<MiPropuestaListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return new ServiceResponse<PaginatedResponse<MiPropuestaListDto>>
            {
                Data = paginatedResponse,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Propuestas obtenidas exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting propuestas for user {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PaginatedResponse<MiPropuestaListDto>>(
                "Error inesperado al obtener propuestas",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
