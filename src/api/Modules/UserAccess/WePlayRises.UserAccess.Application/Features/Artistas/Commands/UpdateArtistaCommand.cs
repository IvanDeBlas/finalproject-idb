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

namespace WePlayRises.UserAccess.Application.Features.Artistas.Commands;

public class UpdateArtistaCommand : IRequest<ServiceResponse<ArtistaDto>>
{
    public Guid Id { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }

    // Populated from JWT token, NOT from request body
    public string? UserId { get; set; }
}

public class UpdateArtistaCommandHandler : IRequestHandler<UpdateArtistaCommand, ServiceResponse<ArtistaDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateArtistaCommand> _validator;
    private readonly ILogger<UpdateArtistaCommandHandler> _logger;

    public UpdateArtistaCommandHandler(
        IArtistaService artistaService,
        IMapper mapper,
        IValidator<UpdateArtistaCommand> validator,
        ILogger<UpdateArtistaCommandHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ArtistaDto>> Handle(UpdateArtistaCommand request, CancellationToken ct)
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

            // 2. Get existing entity
            var artistaId = new ArtistaId(request.Id);
            var existingArtista = await _artistaService.GetByIdAsync(artistaId, ct);
            if (existingArtista == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<ArtistaDto>(
                    "Perfil de artista no encontrado",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            // 3. Verify ownership
            if (existingArtista.UserIdPropietario != request.UserId)
            {
                return ValidateExtensions.UnauthorizedServiceResponse<ArtistaDto>(
                    "No tienes permisos para modificar este perfil",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            // 4. Update fields
            existingArtista.NombreArtistico = request.NombreArtistico;
            existingArtista.Descripcion = request.Descripcion;
            existingArtista.Pais = request.Pais;
            existingArtista.Ciudad = request.Ciudad;
            existingArtista.ImagenPerfilUrl = request.ImagenUrl;
            existingArtista.FechaActualizacion = DateTime.UtcNow;

            // 5. Update via Service
            await _artistaService.UpdateAsync(existingArtista, ct);

            // 6. Map to DTO
            var dto = _mapper.Map<ArtistaDto>(existingArtista);

            _logger.LogInformation("Artist profile {ArtistaId} updated by user {UserId}",
                request.Id, request.UserId);

            return new ServiceResponse<ArtistaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Perfil de artista actualizado exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Artista {ArtistaId}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<ArtistaDto>(
                "Error inesperado al actualizar perfil",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
