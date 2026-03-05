using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class SendMensajeCommand : IRequest<ServiceResponse<MensajeDto>>
{
    public Guid ConversacionId { get; set; }
    public string UserIdRemitente { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string? UrlAdjunto { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class SendMensajeCommandHandler : IRequestHandler<SendMensajeCommand, ServiceResponse<MensajeDto>>
{
    private readonly IConversacionCrowdsourcingService _conversacionService;
    private readonly IMensajeCrowdsourcingService _mensajeService;
    private readonly IArtistaService _artistaService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IValidator<SendMensajeCommand> _validator;
    private readonly ILogger<SendMensajeCommandHandler> _logger;

    public SendMensajeCommandHandler(
        IConversacionCrowdsourcingService conversacionService,
        IMensajeCrowdsourcingService mensajeService,
        IArtistaService artistaService,
        IPerfilProfesionalService perfilService,
        IValidator<SendMensajeCommand> validator,
        ILogger<SendMensajeCommandHandler> logger)
    {
        _conversacionService = conversacionService ?? throw new ArgumentNullException(nameof(conversacionService));
        _mensajeService = mensajeService ?? throw new ArgumentNullException(nameof(mensajeService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<MensajeDto>> Handle(
        SendMensajeCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<MensajeDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var conversacion = await _conversacionService.GetByIdAsync(request.ConversacionId, ct);
            if (conversacion == null)
            {
                return new ServiceResponse<MensajeDto>
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

            bool esParticipante = conversacion.UserIdCreador == request.UserIdRemitente
                                  || conversacion.UserIdDestinatario == request.UserIdRemitente;
            if (!esParticipante)
            {
                return new ServiceResponse<MensajeDto>
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

            var entity = new MensajeCrowdsourcing
            {
                Id = Guid.NewGuid(),
                ConversacionId = request.ConversacionId,
                UserIdRemitente = request.UserIdRemitente,
                Contenido = request.Contenido,
                UrlAdjunto = request.UrlAdjunto,
                Leido = false,
                FechaLeido = null,
                FechaCreacion = DateTime.UtcNow
            };

            var mensajeCreado = await _mensajeService.SendAsync(entity, request.ConversacionId, ct);

            // Resolve RemitenteNombre
            string remitenteNombre;
            var artista = await _artistaService.GetByUserIdAsync(request.UserIdRemitente, ct);
            if (artista != null)
                remitenteNombre = artista.NombreArtistico;
            else
            {
                var perfil = await _perfilService.GetByUserIdAsync(request.UserIdRemitente, ct);
                remitenteNombre = perfil?.Titulo ?? string.Empty;
            }

            var dto = new MensajeDto
            {
                Id = mensajeCreado.Id,
                Contenido = mensajeCreado.Contenido,
                UrlAdjunto = mensajeCreado.UrlAdjunto,
                RemitenteNombre = remitenteNombre,
                EsPropio = true,
                Leido = false,
                FechaCreacion = mensajeCreado.FechaCreacion
            };

            return new ServiceResponse<MensajeDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Mensaje enviado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending mensaje in conversacion {ConversacionId} by user {UserIdRemitente}",
                request.ConversacionId, request.UserIdRemitente);
            return ValidateExtensions.InternalServerErrorServiceResponse<MensajeDto>(
                "Error inesperado al enviar el mensaje",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
