using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetDashboardMisCampaniasQuery : IRequest<ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>>
{
    public string UserId { get; set; } = null!;
    public int? EstadoCampaniaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetDashboardMisCampaniasQueryHandler
    : IRequestHandler<GetDashboardMisCampaniasQuery, ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>>
{
    private readonly IArtistaService _artistaService;
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IValidator<GetDashboardMisCampaniasQuery> _validator;
    private readonly ILogger<GetDashboardMisCampaniasQueryHandler> _logger;

    public GetDashboardMisCampaniasQueryHandler(
        IArtistaService artistaService,
        ICampaniaRepository campaniaRepository,
        IPedidoRepository pedidoRepository,
        IValidator<GetDashboardMisCampaniasQuery> validator,
        ILogger<GetDashboardMisCampaniasQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>> Handle(
        GetDashboardMisCampaniasQuery request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>(
                    "Artista no encontrado",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            // CampaniasController stores userId as ArtistaId in campanias
            var userArtistaId = new ArtistaId(Guid.Parse(request.UserId));

            var campanias = await _campaniaRepository.GetMisCampaniasPaginatedAsync(
                userArtistaId, request.EstadoCampaniaId, request.Page, request.PageSize, ct);

            var totalCount = await _campaniaRepository.CountMisCampaniasAsync(
                userArtistaId, request.EstadoCampaniaId, ct);

            var items = new List<MiCampaniaListItemDto>();
            foreach (var campania in campanias)
            {
                var numBackers = await _pedidoRepository.CountByCampaniaIdAsync(campania.Id, ct);

                items.Add(new MiCampaniaListItemDto
                {
                    Id = campania.Id.Value,
                    Titulo = campania.Titulo,
                    ImagenPrincipalUrl = campania.ImagenPrincipalUrl,
                    EstadoCampaniaId = campania.EstadoCampaniaId,
                    EstadoCampaniaNombre = GetEstadoNombre(campania.EstadoCampaniaId),
                    ImporteObjetivo = campania.ImporteObjetivo,
                    ImporteRecaudado = campania.ImportePledgedActual,
                    PorcentajeProgreso = campania.ImporteObjetivo > 0
                        ? Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 2)
                        : 0,
                    NumBackers = numBackers,
                    DiasRestantes = campania.FechaFin.HasValue
                        ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
                        : null,
                    FechaFin = campania.FechaFin,
                    FechaCreacion = campania.FechaCreacion
                });
            }

            var response = new PaginatedResponse<MiCampaniaListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return new ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>
            {
                Data = response,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Campanias obtenidas", HttpStatusCode = System.Net.HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard mis campanias for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }

    private static string GetEstadoNombre(int estadoId)
    {
        return estadoId switch
        {
            1 => "Borrador",
            2 => "Publicada",
            3 => "Finalizada",
            4 => "Cancelada",
            5 => "Pausada",
            _ => "Desconocido"
        };
    }
}
