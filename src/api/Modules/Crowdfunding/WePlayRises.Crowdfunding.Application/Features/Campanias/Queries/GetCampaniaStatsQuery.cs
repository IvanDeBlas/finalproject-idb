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
public class GetCampaniaStatsQuery : IRequest<ServiceResponse<CampaniaStatsDto>>
{
    public Guid CampaniaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetCampaniaStatsQueryHandler : IRequestHandler<GetCampaniaStatsQuery, ServiceResponse<CampaniaStatsDto>>
{
    private readonly ICampaniaService _campaniaService;
    private readonly IPedidoService _pedidoService;
    private readonly ILogger<GetCampaniaStatsQueryHandler> _logger;

    public GetCampaniaStatsQueryHandler(
        ICampaniaService campaniaService,
        IPedidoService pedidoService,
        ILogger<GetCampaniaStatsQueryHandler> logger)
    {
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CampaniaStatsDto>> Handle(
        GetCampaniaStatsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);

            if (campania == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CampaniaStatsDto>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            var totalBackers = await _pedidoService.CountByCampaniaIdAsync(campaniaId, cancellationToken);
            var stats = await _pedidoService.GetStatsByCampaniaIdAsync(campaniaId, cancellationToken);

            var dto = new CampaniaStatsDto
            {
                CampaniaId = request.CampaniaId,
                TotalBackers = totalBackers,
                TotalRecaudado = stats.Total,
                PromedioAporte = stats.Average,
                AporteMinimo = stats.Min,
                AporteMaximo = stats.Max,
                DiasRestantes = campania.FechaFin.HasValue
                    ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
                    : 0
            };

            return new ServiceResponse<CampaniaStatsDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Estadisticas obtenidas",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stats for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<CampaniaStatsDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
