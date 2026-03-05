using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;

public class GetPromotorMetricasQuery : IRequest<ServiceResponse<PromotorMetricasResponseDto>>
{
    public string UserId { get; set; } = null!;
    public Guid? ProgramaId { get; set; }
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
}

public class GetPromotorMetricasQueryHandler
    : IRequestHandler<GetPromotorMetricasQuery, ServiceResponse<PromotorMetricasResponseDto>>
{
    private readonly IPromoEventoService _promoEventoService;
    private readonly IValidator<GetPromotorMetricasQuery> _validator;
    private readonly ILogger<GetPromotorMetricasQueryHandler> _logger;

    public GetPromotorMetricasQueryHandler(
        IPromoEventoService promoEventoService,
        IValidator<GetPromotorMetricasQuery> validator,
        ILogger<GetPromotorMetricasQueryHandler> logger)
    {
        _promoEventoService = promoEventoService ?? throw new ArgumentNullException(nameof(promoEventoService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromotorMetricasResponseDto>> Handle(
        GetPromotorMetricasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PromotorMetricasResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolve Promotor
            var promotor = await _promoEventoService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorMetricasResponseDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            // 3. Calculate date defaults
            var fechaDesde = request.FechaDesde ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90));
            var fechaHasta = request.FechaHasta ?? DateOnly.FromDateTime(DateTime.UtcNow);

            // 4. Get metrics
            var metricasDto = await _promoEventoService.GetPromotorMetricasAsync(
                promotor.Id, request.ProgramaId, fechaDesde, fechaHasta, cancellationToken);

            return new ServiceResponse<PromotorMetricasResponseDto>
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
            _logger.LogError(ex, "Error obteniendo metricas del promotor para usuario {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromotorMetricasResponseDto>(
                "Error inesperado al obtener las metricas",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
