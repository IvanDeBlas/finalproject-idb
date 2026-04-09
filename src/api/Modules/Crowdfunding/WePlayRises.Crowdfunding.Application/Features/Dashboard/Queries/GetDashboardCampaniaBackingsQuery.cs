using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetDashboardCampaniaBackingsQuery : IRequest<ServiceResponse<CampaniaBackingListDto>>
{
    public Guid CampaniaId { get; set; }
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetDashboardCampaniaBackingsQueryHandler
    : IRequestHandler<GetDashboardCampaniaBackingsQuery, ServiceResponse<CampaniaBackingListDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly ICampaniaService _campaniaService;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IDashboardService _dashboardService;
    private readonly IValidator<GetDashboardCampaniaBackingsQuery> _validator;
    private readonly ILogger<GetDashboardCampaniaBackingsQueryHandler> _logger;

    public GetDashboardCampaniaBackingsQueryHandler(
        IArtistaService artistaService,
        ICampaniaService campaniaService,
        IPedidoRepository pedidoRepository,
        IDashboardService dashboardService,
        IValidator<GetDashboardCampaniaBackingsQuery> validator,
        ILogger<GetDashboardCampaniaBackingsQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CampaniaBackingListDto>> Handle(
        GetDashboardCampaniaBackingsQuery request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<CampaniaBackingListDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CampaniaBackingListDto>(
                    "Artista no encontrado",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            // CampaniasController stores userId as ArtistaId in campanias
            var userArtistaId = new ArtistaId(Guid.Parse(request.UserId));

            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campania = await _campaniaService.GetByIdAsync(campaniaId, ct);
            if (campania == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CampaniaBackingListDto>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            if (campania.ArtistaId != userArtistaId)
            {
                return ValidateExtensions.ForbiddenServiceResponse<CampaniaBackingListDto>(
                    "No tienes permiso para ver estos aportes",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            var pedidos = await _pedidoRepository.GetByCampaniaIdPaginatedAsync(
                campaniaId, request.Page, request.PageSize, ct);
            var totalCount = await _pedidoRepository.CountByCampaniaIdAsync(campaniaId, ct);

            var backingItems = pedidos.Select(p => new CampaniaBackingItemDto
            {
                Id = p.Id.Value,
                NombreBacker = p.PermitirMostrarNombre ? "Backer" : "Anonimo",
                Email = null,
                Monto = p.ImporteTotal,
                RewardNombre = p.Lineas.FirstOrDefault()?.Reward?.Nombre,
                Mensaje = p.ComentarioBacker,
                EsAnonimo = !p.PermitirMostrarNombre,
                EstadoPedido = "Completado",
                FechaCreacion = p.FechaCreacion
            }).ToList();

            var backingPromedio = await _dashboardService.GetBackingPromedioAsync(campaniaId, ct);
            var rewardMasPopular = await _dashboardService.GetRewardMasPopularAsync(campaniaId, ct);
            var ultimoPedido = await _pedidoRepository.GetLastByCampaniaIdAsync(campaniaId, ct);

            var stats = new CampaniaBackingStatsDto
            {
                TotalRecaudado = campania.ImportePledgedActual,
                BackingPromedio = backingPromedio,
                TotalBackers = totalCount,
                RewardMasPopular = rewardMasPopular,
                UltimoBacking = ultimoPedido != null
                    ? new UltimoBackingDto
                    {
                        NombreBacker = ultimoPedido.PermitirMostrarNombre ? "Backer" : "Anonimo",
                        Monto = ultimoPedido.ImporteTotal,
                        FechaCreacion = ultimoPedido.FechaCreacion
                    }
                    : null
            };

            var dto = new CampaniaBackingListDto
            {
                CampaniaId = request.CampaniaId,
                CampaniaTitulo = campania.Titulo,
                Stats = stats,
                Backings = new PaginatedResponse<CampaniaBackingItemDto>
                {
                    Items = backingItems,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                }
            };

            return new ServiceResponse<CampaniaBackingListDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Backings obtenidos", HttpStatusCode = System.Net.HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backings for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<CampaniaBackingListDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
