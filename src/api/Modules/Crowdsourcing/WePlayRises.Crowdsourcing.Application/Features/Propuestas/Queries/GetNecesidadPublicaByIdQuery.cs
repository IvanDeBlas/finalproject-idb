using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetNecesidadPublicaByIdQuery : IRequest<ServiceResponse<NecesidadPublicaDto>>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetNecesidadPublicaByIdQueryHandler : IRequestHandler<GetNecesidadPublicaByIdQuery, ServiceResponse<NecesidadPublicaDto>>
{
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IPropuestaCrowdsourcingService _propuestaService;
    private readonly IArtistaService _artistaService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetNecesidadPublicaByIdQueryHandler> _logger;

    public GetNecesidadPublicaByIdQueryHandler(
        INecesidadCrowdsourcingService necesidadService,
        IPropuestaCrowdsourcingService propuestaService,
        IArtistaService artistaService,
        IPerfilProfesionalService perfilService,
        IMapper mapper,
        ILogger<GetNecesidadPublicaByIdQueryHandler> logger)
    {
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<NecesidadPublicaDto>> Handle(
        GetNecesidadPublicaByIdQuery request,
        CancellationToken ct)
    {
        try
        {
            var necesidadId = new NecesidadCrowdsourcingId(request.Id);
            var entity = await _necesidadService.GetPublicaByIdAsync(necesidadId, ct);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<NecesidadPublicaDto>(
                    "Necesidad no encontrada",
                    ServiceResponseMessageType.NotFound_Necesidad);
            }

            var yaPropuso = await _propuestaService.ExistePropuestaActivaAsync(necesidadId, request.UserId, ct);

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            var esPropietario = artista != null && entity.ArtistaId == artista.Id;

            var tienePerfilProfesional = await _perfilService.ExistsByUserIdAsync(request.UserId, ct);

            var artistaPropietario = await _artistaService.GetByIdAsync(entity.ArtistaId, ct);

            var dto = _mapper.Map<NecesidadPublicaDto>(entity);

            dto.YaPropuso = yaPropuso;
            dto.EsPropietario = esPropietario;
            dto.TienePerfilProfesional = tienePerfilProfesional;
            dto.EstadoNecesidadNombre = "Abierta";
            dto.NumeroPropuestas = entity.Propuestas?.Count(p =>
                p.EstadoPropuestaId != EstadoPropuestaConstants.Retirada) ?? 0;
            dto.Artista = new ArtistaPublicoDto
            {
                Id = artistaPropietario?.Id.Value ?? entity.ArtistaId.Value,
                NombreArtistico = artistaPropietario?.NombreArtistico ?? string.Empty,
                ImagenUrl = artistaPropietario?.ImagenPerfilUrl
            };

            return new ServiceResponse<NecesidadPublicaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidad obtenida exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting necesidad publica {NecesidadId}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadPublicaDto>(
                "Error inesperado al obtener necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
