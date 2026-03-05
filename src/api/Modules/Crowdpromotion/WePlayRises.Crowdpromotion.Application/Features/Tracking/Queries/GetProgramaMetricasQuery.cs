using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;

public class GetProgramaMetricasQuery : IRequest<ServiceResponse<ProgramaMetricasResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public string UserId { get; set; } = null!;
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
}

public class GetProgramaMetricasQueryHandler
    : IRequestHandler<GetProgramaMetricasQuery, ServiceResponse<ProgramaMetricasResponseDto>>
{
    private readonly IPromoEventoService _promoEventoService;
    private readonly IPromoProgramaService _promoProgramaService;
    private readonly IValidator<GetProgramaMetricasQuery> _validator;
    private readonly ILogger<GetProgramaMetricasQueryHandler> _logger;

    public GetProgramaMetricasQueryHandler(
        IPromoEventoService promoEventoService,
        IPromoProgramaService promoProgramaService,
        IValidator<GetProgramaMetricasQuery> validator,
        ILogger<GetProgramaMetricasQueryHandler> logger)
    {
        _promoEventoService = promoEventoService ?? throw new ArgumentNullException(nameof(promoEventoService));
        _promoProgramaService = promoProgramaService ?? throw new ArgumentNullException(nameof(promoProgramaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ProgramaMetricasResponseDto>> Handle(
        GetProgramaMetricasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<ProgramaMetricasResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolve Artista
            var artistaId = await _promoProgramaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<ProgramaMetricasResponseDto>(
                    "No tienes un perfil de artista",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            // 3. Verify programa exists
            var programa = await _promoEventoService.GetProgramaByIdAsync(request.ProgramaId, cancellationToken);
            if (programa == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<ProgramaMetricasResponseDto>(
                    "El programa de promocion no existe",
                    ServiceResponseMessageType.NotFound_PromoPrograma);
            }

            // 4. Verify ownership
            if (programa.ArtistaId != artistaId)
            {
                return ValidateExtensions.ForbiddenServiceResponse<ProgramaMetricasResponseDto>(
                    "No tienes permiso para ver las metricas de este programa",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);
            }

            // 5. Calculate date defaults
            var fechaDesde = request.FechaDesde ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90));
            var fechaHasta = request.FechaHasta ?? DateOnly.FromDateTime(DateTime.UtcNow);

            // 6. Get metrics
            var metricasDto = await _promoEventoService.GetProgramaMetricasAsync(
                request.ProgramaId, fechaDesde, fechaHasta, cancellationToken);

            return new ServiceResponse<ProgramaMetricasResponseDto>
            {
                Data = metricasDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Metricas obtenidas", HttpStatusCode = System.Net.HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo metricas del programa {ProgramaId} para usuario {UserId}",
                request.ProgramaId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ProgramaMetricasResponseDto>(
                "Error inesperado al obtener las metricas",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
