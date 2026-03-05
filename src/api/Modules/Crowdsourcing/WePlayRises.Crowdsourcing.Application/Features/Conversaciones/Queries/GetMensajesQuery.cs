using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetMensajesQuery : IRequest<ServiceResponse<MensajeListResponseDto>>
{
    public Guid ConversacionId { get; set; }
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetMensajesQueryHandler : IRequestHandler<GetMensajesQuery, ServiceResponse<MensajeListResponseDto>>
{
    private readonly IConversacionCrowdsourcingService _conversacionService;
    private readonly IMensajeCrowdsourcingService _mensajeService;
    private readonly IArtistaService _artistaService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IValidator<GetMensajesQuery> _validator;
    private readonly ILogger<GetMensajesQueryHandler> _logger;

    public GetMensajesQueryHandler(
        IConversacionCrowdsourcingService conversacionService,
        IMensajeCrowdsourcingService mensajeService,
        IArtistaService artistaService,
        IPerfilProfesionalService perfilService,
        IValidator<GetMensajesQuery> validator,
        ILogger<GetMensajesQueryHandler> logger)
    {
        _conversacionService = conversacionService ?? throw new ArgumentNullException(nameof(conversacionService));
        _mensajeService = mensajeService ?? throw new ArgumentNullException(nameof(mensajeService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<MensajeListResponseDto>> Handle(
        GetMensajesQuery request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<MensajeListResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var conversacion = await _conversacionService.GetByIdAsync(request.ConversacionId, ct);
            if (conversacion == null)
            {
                return new ServiceResponse<MensajeListResponseDto>
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
                return new ServiceResponse<MensajeListResponseDto>
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

            var pageSize = Math.Min(request.PageSize, 100);
            var (mensajes, totalCount) = await _mensajeService.GetByConversacionIdPaginatedAsync(
                request.ConversacionId, request.Page, pageSize, ct);

            // Cache to avoid N+1 for user name resolution
            var nombreCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var dtos = new List<MensajeDto>();
            foreach (var mensaje in mensajes)
            {
                if (!nombreCache.TryGetValue(mensaje.UserIdRemitente, out var remitenteNombre))
                {
                    var artista = await _artistaService.GetByUserIdAsync(mensaje.UserIdRemitente, ct);
                    if (artista != null)
                        remitenteNombre = artista.NombreArtistico;
                    else
                    {
                        var perfil = await _perfilService.GetByUserIdAsync(mensaje.UserIdRemitente, ct);
                        remitenteNombre = perfil?.Titulo ?? string.Empty;
                    }
                    nombreCache[mensaje.UserIdRemitente] = remitenteNombre;
                }

                dtos.Add(new MensajeDto
                {
                    Id = mensaje.Id,
                    Contenido = mensaje.Contenido,
                    UrlAdjunto = mensaje.UrlAdjunto,
                    RemitenteNombre = remitenteNombre,
                    EsPropio = mensaje.UserIdRemitente == request.UserId,
                    Leido = mensaje.Leido,
                    FechaCreacion = mensaje.FechaCreacion
                });
            }

            return new ServiceResponse<MensajeListResponseDto>
            {
                Data = new MensajeListResponseDto
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = pageSize
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mensajes for conversacion {ConversacionId} by user {UserId}",
                request.ConversacionId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<MensajeListResponseDto>(
                "Error inesperado al obtener los mensajes",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
