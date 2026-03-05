using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class MarcarLeidosCommand : IRequest<ServiceResponse<MarcarLeidosResponseDto>>
{
    public Guid ConversacionId { get; set; }
    public string UserId { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class MarcarLeidosCommandHandler : IRequestHandler<MarcarLeidosCommand, ServiceResponse<MarcarLeidosResponseDto>>
{
    private readonly IConversacionCrowdsourcingService _conversacionService;
    private readonly IMensajeCrowdsourcingService _mensajeService;
    private readonly IValidator<MarcarLeidosCommand> _validator;
    private readonly ILogger<MarcarLeidosCommandHandler> _logger;

    public MarcarLeidosCommandHandler(
        IConversacionCrowdsourcingService conversacionService,
        IMensajeCrowdsourcingService mensajeService,
        IValidator<MarcarLeidosCommand> validator,
        ILogger<MarcarLeidosCommandHandler> logger)
    {
        _conversacionService = conversacionService ?? throw new ArgumentNullException(nameof(conversacionService));
        _mensajeService = mensajeService ?? throw new ArgumentNullException(nameof(mensajeService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<MarcarLeidosResponseDto>> Handle(
        MarcarLeidosCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<MarcarLeidosResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var conversacion = await _conversacionService.GetByIdAsync(request.ConversacionId, ct);
            if (conversacion == null)
            {
                return new ServiceResponse<MarcarLeidosResponseDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "La conversacion no existe",
                            ErrorCode = ServiceResponseMessageType.NotFound_Conversacion
                        }
                    }
                };
            }

            bool esParticipante = conversacion.UserIdCreador == request.UserId
                                  || conversacion.UserIdDestinatario == request.UserId;
            if (!esParticipante)
            {
                return new ServiceResponse<MarcarLeidosResponseDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "No tienes acceso a esta conversacion",
                            ErrorCode = ServiceResponseMessageType.Auth_Forbidden
                        }
                    }
                };
            }

            var mensajesMarcados = await _mensajeService.MarcarLeidosAsync(
                request.ConversacionId, request.UserId, ct);

            return new ServiceResponse<MarcarLeidosResponseDto>
            {
                Data = new MarcarLeidosResponseDto { MensajesMarcados = mensajesMarcados }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking mensajes as read in conversacion {ConversacionId} by user {UserId}",
                request.ConversacionId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<MarcarLeidosResponseDto>(
                "Error inesperado al marcar mensajes como leidos",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
