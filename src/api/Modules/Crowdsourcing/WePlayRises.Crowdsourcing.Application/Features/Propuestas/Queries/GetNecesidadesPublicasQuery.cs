using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetNecesidadesPublicasQuery : IRequest<ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>>
{
    public string UserId { get; set; } = null!;
    public int? TipoNecesidadId { get; set; }
    public int? ModalidadTrabajoId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public string? Pais { get; set; }
    public string? Search { get; set; }
    public string? OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetNecesidadesPublicasQueryHandler : IRequestHandler<GetNecesidadesPublicasQuery, ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetNecesidadesPublicasQueryHandler> _logger;

    public GetNecesidadesPublicasQueryHandler(
        INecesidadCrowdsourcingService service,
        IArtistaService artistaService,
        IMapper mapper,
        ILogger<GetNecesidadesPublicasQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>> Handle(
        GetNecesidadesPublicasQuery request,
        CancellationToken ct)
    {
        try
        {
            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            var artistaId = artista?.Id;

            var filtro = new NecesidadesPublicasFiltro(
                TiposNecesidadIds: request.TipoNecesidadId.HasValue ? new[] { request.TipoNecesidadId.Value } : null,
                ModalidadTrabajoId: request.ModalidadTrabajoId,
                PresupuestoMin: request.PresupuestoMin,
                PresupuestoMax: request.PresupuestoMax,
                Pais: request.Pais,
                Search: request.Search,
                OrderBy: request.OrderBy ?? "recientes",
                Page: request.Page,
                PageSize: Math.Min(request.PageSize, 50)
            );

            var (items, totalCount) = await _service.GetPublicasPaginatedAsync(filtro, artistaId, ct);

            var dtos = _mapper.Map<List<NecesidadPublicaListDto>>(items);

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var dto = dtos[i];

                var itemArtista = await _artistaService.GetByIdAsync(item.ArtistaId, ct);
                dto.ArtistaNombre = itemArtista?.NombreArtistico ?? string.Empty;

                dto.EsUrgente = item.FechaLimitePropuestas.HasValue
                    && item.FechaLimitePropuestas.Value <= DateTime.UtcNow.AddDays(3);

                dto.NumeroPropuestas = item.Propuestas?.Count(p =>
                    p.EstadoPropuestaId != EstadoPropuestaConstants.Retirada) ?? 0;
            }

            var paginatedResponse = new PaginatedResponse<NecesidadPublicaListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return new ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>
            {
                Data = paginatedResponse,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidades obtenidas exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting necesidades publicas for user {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>(
                "Error inesperado al obtener necesidades",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
