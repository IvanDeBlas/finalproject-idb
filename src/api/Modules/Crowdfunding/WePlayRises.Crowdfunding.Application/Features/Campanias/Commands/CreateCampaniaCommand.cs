using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateCampaniaCommand : IRequest<ServiceResponse<CampaniaDto>>
{
    public Guid ArtistaId { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public int MonedaId { get; set; }
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int TipoFinanciacionId { get; set; }
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateCampaniaCommandHandler : IRequestHandler<CreateCampaniaCommand, ServiceResponse<CampaniaDto>>
{
    private readonly ICampaniaService _service;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCampaniaCommand> _validator;
    private readonly ILogger<CreateCampaniaCommandHandler> _logger;

    public CreateCampaniaCommandHandler(
        ICampaniaService service,
        IMapper mapper,
        IValidator<CreateCampaniaCommand> validator,
        ILogger<CreateCampaniaCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CampaniaDto>> Handle(
        CreateCampaniaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateCampania: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<CampaniaDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Mapear a entidad de dominio
            var entity = _mapper.Map<CampaniaCrowdfunding>(request);
            entity.Id = CampaniaCrowdfundingId.CreateNew();
            entity.EstadoCampaniaId = 1; // Borrador
            entity.ImportePledgedActual = 0;
            entity.FechaCreacion = DateTime.UtcNow;

            // 3. Crear via servicio
            var id = await _service.CreateAsync(entity, cancellationToken);

            // 4. Obtener entidad creada y mapear a DTO
            var created = await _service.GetByIdAsync(id, cancellationToken);
            var dto = _mapper.Map<CampaniaDto>(created);

            _logger.LogInformation("Campania created with Id {Id} for Artista {ArtistaId}",
                id.Value, request.ArtistaId);

            // 5. Retornar respuesta exitosa
            return new ServiceResponse<CampaniaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campania creada correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Campania for Artista {ArtistaId}", request.ArtistaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<CampaniaDto>(
                "Error inesperado al crear campania",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
