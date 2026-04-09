using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Artistas.Queries;

public class GetArtistaByUserIdQuery : IRequest<ServiceResponse<ArtistaDto>>
{
    public string UserId { get; set; } = null!;

    // Populated from JWT token for authorization validation
    public string? RequestingUserId { get; set; }
}

public class GetArtistaByUserIdQueryHandler : IRequestHandler<GetArtistaByUserIdQuery, ServiceResponse<ArtistaDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetArtistaByUserIdQueryHandler> _logger;

    public GetArtistaByUserIdQueryHandler(
        IArtistaService artistaService,
        IMapper mapper,
        ILogger<GetArtistaByUserIdQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ArtistaDto>> Handle(GetArtistaByUserIdQuery request, CancellationToken ct)
    {
        try
        {
            // 1. Authorization check
            if (string.IsNullOrEmpty(request.RequestingUserId) ||
                request.RequestingUserId != request.UserId)
            {
                return ValidateExtensions.UnauthorizedServiceResponse<ArtistaDto>(
                    "No tienes permisos para ver este perfil",
                    ServiceResponseMessageType.Auth_Unauthorized);
            }

            // 2. Get artista by UserId
            var entity = await _artistaService.GetByUserIdAsync(request.UserId, ct);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<ArtistaDto>(
                    "Artista no encontrado para este usuario",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            var dto = _mapper.Map<ArtistaDto>(entity);

            return new ServiceResponse<ArtistaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Artista encontrado",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Artista by UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ArtistaDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
