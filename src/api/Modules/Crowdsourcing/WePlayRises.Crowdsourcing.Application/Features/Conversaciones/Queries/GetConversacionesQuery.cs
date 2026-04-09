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
public class GetConversacionesQuery : IRequest<ServiceResponse<ConversacionListResponseDto>>
{
    public string UserId { get; set; } = null!;
    public string Contexto { get; set; } = "todas";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetConversacionesQueryHandler : IRequestHandler<GetConversacionesQuery, ServiceResponse<ConversacionListResponseDto>>
{
    private readonly IConversacionCrowdsourcingService _conversacionService;
    private readonly IArtistaService _artistaService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IValidator<GetConversacionesQuery> _validator;
    private readonly ILogger<GetConversacionesQueryHandler> _logger;

    public GetConversacionesQueryHandler(
        IConversacionCrowdsourcingService conversacionService,
        IArtistaService artistaService,
        IPerfilProfesionalService perfilService,
        IValidator<GetConversacionesQuery> validator,
        ILogger<GetConversacionesQueryHandler> logger)
    {
        _conversacionService = conversacionService ?? throw new ArgumentNullException(nameof(conversacionService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ConversacionListResponseDto>> Handle(
        GetConversacionesQuery request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<ConversacionListResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var pageSize = Math.Min(request.PageSize, 50);
            var (items, totalCount, totalNoLeidos) = await _conversacionService.GetConversacionesByUserIdAsync(
                request.UserId, request.Contexto, request.Page, pageSize, ct);

            // Cache to avoid N+1 for user profile resolution
            var perfilCache = new Dictionary<string, (string Nombre, string? Imagen)>(StringComparer.OrdinalIgnoreCase);

            var dtos = new List<ConversacionListItemDto>();
            foreach (var conv in items)
            {
                var userIdOtraParte = conv.UserIdCreador == request.UserId
                    ? conv.UserIdDestinatario
                    : conv.UserIdCreador;

                if (!perfilCache.TryGetValue(userIdOtraParte, out var perfilOtraParte))
                {
                    string nombre;
                    string? imagen = null;
                    var artista = await _artistaService.GetByUserIdAsync(userIdOtraParte, ct);
                    if (artista != null)
                    {
                        nombre = artista.NombreArtistico;
                        imagen = artista.ImagenPerfilUrl;
                    }
                    else
                    {
                        var perfil = await _perfilService.GetByUserIdAsync(userIdOtraParte, ct);
                        nombre = perfil?.Titulo ?? string.Empty;
                    }
                    perfilOtraParte = (nombre, imagen);
                    perfilCache[userIdOtraParte] = perfilOtraParte;
                }

                string contextoTipo = conv.NecesidadId.HasValue ? "necesidad" : "acuerdo";
                string contextoTitulo = conv.NecesidadId.HasValue
                    ? conv.Necesidad?.Titulo ?? string.Empty
                    : conv.Acuerdo?.Descripcion ?? string.Empty;

                // Calculate unread messages from loaded Mensajes collection
                int mensajesNoLeidos = conv.Mensajes?
                    .Count(m => m.UserIdRemitente != request.UserId && !m.Leido) ?? 0;

                // Get last message preview (truncated to 80 chars)
                var ultimoMensaje = conv.Mensajes?
                    .OrderByDescending(m => m.FechaCreacion)
                    .FirstOrDefault();
                string? ultimoMensajePreview = ultimoMensaje?.Contenido;
                if (ultimoMensajePreview != null && ultimoMensajePreview.Length > 80)
                    ultimoMensajePreview = ultimoMensajePreview[..80];

                var dto = new ConversacionListItemDto
                {
                    Id = conv.Id,
                    Asunto = conv.Asunto,
                    NombreOtraParte = perfilOtraParte.Nombre,
                    ImagenOtraParte = perfilOtraParte.Imagen,
                    ContextoTipo = contextoTipo,
                    ContextoTitulo = contextoTitulo,
                    UltimoMensaje = ultimoMensajePreview,
                    FechaUltimoMensaje = conv.FechaUltimoMensaje,
                    MensajesNoLeidos = mensajesNoLeidos
                };
                dtos.Add(dto);
            }

            var response = new ConversacionListResponseDto
            {
                Items = dtos,
                TotalCount = totalCount,
                TotalNoLeidos = totalNoLeidos,
                Page = request.Page,
                PageSize = pageSize
            };

            return new ServiceResponse<ConversacionListResponseDto> { Data = response };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversaciones for user {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ConversacionListResponseDto>(
                "Error inesperado al obtener las conversaciones",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
