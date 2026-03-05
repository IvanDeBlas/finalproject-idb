using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetNoLeidosCountQuery : IRequest<ServiceResponse<NoLeidosCountResponseDto>>
{
    public string UserId { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetNoLeidosCountQueryHandler : IRequestHandler<GetNoLeidosCountQuery, ServiceResponse<NoLeidosCountResponseDto>>
{
    private readonly IMensajeCrowdsourcingService _mensajeService;
    private readonly IValidator<GetNoLeidosCountQuery> _validator;
    private readonly ILogger<GetNoLeidosCountQueryHandler> _logger;

    public GetNoLeidosCountQueryHandler(
        IMensajeCrowdsourcingService mensajeService,
        IValidator<GetNoLeidosCountQuery> validator,
        ILogger<GetNoLeidosCountQueryHandler> logger)
    {
        _mensajeService = mensajeService ?? throw new ArgumentNullException(nameof(mensajeService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<NoLeidosCountResponseDto>> Handle(
        GetNoLeidosCountQuery request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<NoLeidosCountResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var totalNoLeidos = await _mensajeService.GetTotalNoLeidosAsync(request.UserId, ct);

            return new ServiceResponse<NoLeidosCountResponseDto>
            {
                Data = new NoLeidosCountResponseDto { TotalNoLeidos = totalNoLeidos }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting no leidos count for user {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<NoLeidosCountResponseDto>(
                "Error inesperado al obtener el conteo de no leidos",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
