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
public class GetDashboardCampaniaStatsQuery : IRequest<ServiceResponse<CampaniaStatsDetailDto>>
{
    public Guid CampaniaId { get; set; }
    public string UserId { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetDashboardCampaniaStatsQueryHandler
    : IRequestHandler<GetDashboardCampaniaStatsQuery, ServiceResponse<CampaniaStatsDetailDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly ICampaniaService _campaniaService;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IDashboardService _dashboardService;
    private readonly IRewardRepository _rewardRepository;
    private readonly IValidator<GetDashboardCampaniaStatsQuery> _validator;
    private readonly ILogger<GetDashboardCampaniaStatsQueryHandler> _logger;

    public GetDashboardCampaniaStatsQueryHandler(
        IArtistaService artistaService,
        ICampaniaService campaniaService,
        IPedidoRepository pedidoRepository,
        IDashboardService dashboardService,
        IRewardRepository rewardRepository,
        IValidator<GetDashboardCampaniaStatsQuery> validator,
        ILogger<GetDashboardCampaniaStatsQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _rewardRepository = rewardRepository ?? throw new ArgumentNullException(nameof(rewardRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CampaniaStatsDetailDto>> Handle(
        GetDashboardCampaniaStatsQuery request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<CampaniaStatsDetailDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CampaniaStatsDetailDto>(
                    "Artista no encontrado",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            // CampaniasController stores userId as ArtistaId in campanias
            var userArtistaId = new ArtistaId(Guid.Parse(request.UserId));

            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campania = await _campaniaService.GetByIdAsync(campaniaId, ct);
            if (campania == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CampaniaStatsDetailDto>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            if (campania.ArtistaId != userArtistaId)
            {
                return ValidateExtensions.ForbiddenServiceResponse<CampaniaStatsDetailDto>(
                    "No tienes permiso para ver esta campania",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            var numBackers = await _pedidoRepository.CountByCampaniaIdAsync(campaniaId, ct);
            var backingPromedio = await _dashboardService.GetBackingPromedioAsync(campaniaId, ct);
            var velocidadDiaria = await _dashboardService.GetVelocidadDiariaAsync(campaniaId, ct);
            var proyeccionFinal = await _dashboardService.CalcularProyeccionFinalAsync(campaniaId, ct);

            var rewardStatsRaw = await _pedidoRepository.GetRewardStatsByCampaniaIdAsync(campaniaId, ct);
            var rewards = await _rewardRepository.GetByCampaniaIdAsync(campaniaId, ct);

            var rewardStats = new List<RewardStatDto>();
            foreach (var (rewardId, cantidad) in rewardStatsRaw)
            {
                var reward = rewards.FirstOrDefault(r => r.Id.Value == rewardId);
                var totalRewardRecaudado = reward != null ? cantidad * reward.ImporteMinimo : 0;
                rewardStats.Add(new RewardStatDto
                {
                    RewardId = rewardId,
                    RewardNombre = reward?.Nombre ?? "Desconocido",
                    CantidadVendida = cantidad,
                    TotalRecaudado = totalRewardRecaudado,
                    PorcentajeDelTotal = numBackers > 0
                        ? Math.Round((decimal)cantidad / numBackers * 100, 2)
                        : 0
                });
            }

            var progressoPorDia = await _pedidoRepository.GetProgressByDayAsync(campaniaId, ct);
            decimal acumulado = 0;
            var progressoDtos = progressoPorDia.Select(p =>
            {
                acumulado += p.Total;
                return new ProgressoDiaDto
                {
                    Fecha = p.Date.ToString("yyyy-MM-dd"),
                    NumBackings = p.Count,
                    TotalRecaudado = p.Total,
                    Acumulado = acumulado
                };
            }).ToList();

            var dto = new CampaniaStatsDetailDto
            {
                CampaniaId = request.CampaniaId,
                CampaniaTitulo = campania.Titulo,
                ImporteObjetivo = campania.ImporteObjetivo,
                ImporteRecaudado = campania.ImportePledgedActual,
                PorcentajeProgreso = campania.ImporteObjetivo > 0
                    ? Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 2)
                    : 0,
                NumBackers = numBackers,
                BackingPromedio = backingPromedio,
                DiasRestantes = campania.FechaFin.HasValue
                    ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
                    : null,
                DiasTranscurridos = campania.FechaInicio.HasValue
                    ? (DateTime.UtcNow - campania.FechaInicio.Value).Days
                    : 0,
                TotalDiasCampania = (campania.FechaFin.HasValue && campania.FechaInicio.HasValue)
                    ? (campania.FechaFin.Value - campania.FechaInicio.Value).Days
                    : 0,
                ProyeccionFinal = proyeccionFinal,
                VelocidadDiaria = velocidadDiaria,
                RewardStats = rewardStats,
                ProgressoPorDia = progressoDtos
            };

            return new ServiceResponse<CampaniaStatsDetailDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Estadisticas obtenidas", HttpStatusCode = System.Net.HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stats for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<CampaniaStatsDetailDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
