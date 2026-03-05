using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Constants;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Features.Artistas.Commands;

public class CreateArtistaCommand : IRequest<ServiceResponse<ArtistaDto>>
{
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }

    // Populated from JWT token, NOT from request body
    public string? UserId { get; set; }
}

public class CreateArtistaCommandHandler : IRequestHandler<CreateArtistaCommand, ServiceResponse<ArtistaDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateArtistaCommand> _validator;
    private readonly ILogger<CreateArtistaCommandHandler> _logger;

    public CreateArtistaCommandHandler(
        IArtistaService artistaService,
        IMapper mapper,
        IValidator<CreateArtistaCommand> validator,
        ILogger<CreateArtistaCommandHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ArtistaDto>> Handle(CreateArtistaCommand request, CancellationToken ct)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<ArtistaDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Check UserId is present
            if (string.IsNullOrEmpty(request.UserId))
            {
                return ValidateExtensions.UnauthorizedServiceResponse<ArtistaDto>(
                    "UserId es requerido (debe estar autenticado)",
                    ServiceResponseMessageType.Auth_Unauthorized);
            }

            // 3. Check if user already has an artist profile
            var existingArtista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (existingArtista != null)
            {
                return ValidateExtensions.ConflictServiceResponse<ArtistaDto>(
                    "Este usuario ya tiene un perfil de artista",
                    ServiceResponseMessageType.BusinessRule_ArtistaAlreadyExists);
            }

            // 4. Map Command to Entity
            var entity = _mapper.Map<Artista>(request);
            entity.Id = ArtistaId.CreateNew();

            // 5. Create via Service
            var artistaId = await _artistaService.CreateAsync(entity, ct);

            // 6. Get created entity and map to DTO
            var createdArtista = await _artistaService.GetByIdAsync(artistaId, ct);
            var dto = _mapper.Map<ArtistaDto>(createdArtista);

            _logger.LogInformation("Artist profile created for user {UserId} with ArtistaId {ArtistaId}",
                request.UserId, artistaId.Value);

            return new ServiceResponse<ArtistaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Perfil de artista creado exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Artista for user {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ArtistaDto>(
                "Error inesperado al crear perfil",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
