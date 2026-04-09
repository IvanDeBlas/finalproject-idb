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
public class GetDashboardResumenQuery : IRequest<ServiceResponse<DashboardResumenDto>>
{
    public string UserId { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetDashboardResumenQueryHandler : IRequestHandler<GetDashboardResumenQuery, ServiceResponse<DashboardResumenDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IDashboardService _dashboardService;
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ILogger<GetDashboardResumenQueryHandler> _logger;

    public GetDashboardResumenQueryHandler(
        IArtistaService artistaService,
        IDashboardService dashboardService,
        ICampaniaRepository campaniaRepository,
        IPedidoRepository pedidoRepository,
        ILogger<GetDashboardResumenQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<DashboardResumenDto>> Handle(
        GetDashboardResumenQuery request,
        CancellationToken ct)
    {
        try
        {
            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<DashboardResumenDto>(
                    "Artista no encontrado",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            // CampaniasController stores userId as ArtistaId in campanias
            var userArtistaId = new ArtistaId(Guid.Parse(request.UserId));

            var (totalRecaudado, totalBackers, campaniasActivas, campaniasCompletadas) =
                await _dashboardService.GetResumenAsync(userArtistaId, ct);

            var campanias = await _campaniaRepository.GetByArtistaIdAsync(userArtistaId, ct);
            var totalCampanias = campanias.Count(c => !c.Borrado);

            Domain.Model.PedidoCrowdfunding? ultimoPedido = null;
            foreach (var campania in campanias.Where(c => !c.Borrado))
            {
                var ultimo = await _pedidoRepository.GetLastByCampaniaIdAsync(campania.Id, ct);
                if (ultimo != null && (ultimoPedido == null || ultimo.FechaCreacion > ultimoPedido.FechaCreacion))
                {
                    ultimoPedido = ultimo;
                }
            }

            var dto = new DashboardResumenDto
            {
                ArtistaId = artista.Id.Value,
                NombreArtistico = artista.NombreArtistico,
                TotalRecaudado = totalRecaudado,
                TotalBackers = totalBackers,
                CampaniasActivas = campaniasActivas,
                CampaniasCompletadas = campaniasCompletadas,
                TotalCampanias = totalCampanias,
                MonedaSimbolo = "EUR",
                FechaUltimoAporte = ultimoPedido?.FechaCreacion
            };

            return new ServiceResponse<DashboardResumenDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Resumen obtenido", HttpStatusCode = System.Net.HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard resumen for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<DashboardResumenDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
