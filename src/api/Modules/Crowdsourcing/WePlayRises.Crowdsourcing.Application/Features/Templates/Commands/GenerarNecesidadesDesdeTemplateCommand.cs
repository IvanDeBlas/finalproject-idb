using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Features.Templates.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class GenerarNecesidadesDesdeTemplateCommand : IRequest<ServiceResponse<GenerarNecesidadesResultDto>>
{
    public Guid PlantillaId { get; set; }
    public Guid ProyectoArtisticoId { get; set; }
    public Guid ArtistaId { get; set; }
    public List<NecesidadSeleccionadaDto> NecesidadesSeleccionadas { get; set; } = new();
    public string? UserId { get; set; }
}

public class NecesidadSeleccionadaDto
{
    public Guid PlantillaNecesidadId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int MonedaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GenerarNecesidadesDesdeTemplateCommandHandler : IRequestHandler<GenerarNecesidadesDesdeTemplateCommand, ServiceResponse<GenerarNecesidadesResultDto>>
{
    private readonly IPlantillaProyectoService _plantillaService;
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IMapper _mapper;
    private readonly IValidator<GenerarNecesidadesDesdeTemplateCommand> _validator;
    private readonly ILogger<GenerarNecesidadesDesdeTemplateCommandHandler> _logger;

    // Defaults for NecesidadCrowdsourcing maestras
    private const int EstadoAbierta = 2;
    private const int TipoNecesidadDefault = 1;
    private const int ModalidadRemoto = 1;

    public GenerarNecesidadesDesdeTemplateCommandHandler(
        IPlantillaProyectoService plantillaService,
        INecesidadCrowdsourcingService necesidadService,
        IMapper mapper,
        IValidator<GenerarNecesidadesDesdeTemplateCommand> validator,
        ILogger<GenerarNecesidadesDesdeTemplateCommandHandler> logger)
    {
        _plantillaService = plantillaService ?? throw new ArgumentNullException(nameof(plantillaService));
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<GenerarNecesidadesResultDto>> Handle(
        GenerarNecesidadesDesdeTemplateCommand request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar request
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<GenerarNecesidadesResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Validar autenticacion
            if (string.IsNullOrEmpty(request.UserId))
            {
                return ValidateExtensions.UnauthorizedServiceResponse<GenerarNecesidadesResultDto>(
                    "Usuario no autenticado",
                    ServiceResponseMessageType.Auth_Unauthorized);
            }

            // 3. Obtener plantilla con necesidades
            var plantillaId = new PlantillaProyectoId(request.PlantillaId);
            var plantilla = await _plantillaService.GetByIdWithNecesidadesAsync(plantillaId, ct);
            if (plantilla == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<GenerarNecesidadesResultDto>(
                    "Plantilla no encontrada",
                    ServiceResponseMessageType.NotFound_PlantillaProyecto);
            }

            // 4. Validar que todas las PlantillaNecesidadId existen en la plantilla
            var plantillaNecesidadIds = plantilla.Necesidades
                .Select(n => n.Id.Value)
                .ToHashSet();

            var necesidadesInvalidas = request.NecesidadesSeleccionadas
                .Where(n => !plantillaNecesidadIds.Contains(n.PlantillaNecesidadId))
                .ToList();

            if (necesidadesInvalidas.Count > 0)
            {
                _logger.LogWarning(
                    "Intento de generar necesidades con IDs invalidos para plantilla {PlantillaId}",
                    request.PlantillaId);

                return ValidateExtensions.NotFoundServiceResponse<GenerarNecesidadesResultDto>(
                    "Una o mas necesidades de plantilla no fueron encontradas",
                    ServiceResponseMessageType.NotFound_PlantillaNecesidad);
            }

            // 5. Crear entidades NecesidadCrowdsourcing desde plantilla
            var necesidadesACrear = new List<NecesidadCrowdsourcing>();
            var presupuestoTotalMin = 0m;
            var presupuestoTotalMax = 0m;
            var monedaId = 1;

            var proyectoArtisticoId = new ProyectoArtisticoId(request.ProyectoArtisticoId);
            var artistaId = new ArtistaId(request.ArtistaId);

            foreach (var itemSeleccionado in request.NecesidadesSeleccionadas)
            {
                var plantillaNecesidad = plantilla.Necesidades
                    .FirstOrDefault(n => n.Id.Value == itemSeleccionado.PlantillaNecesidadId);

                if (plantillaNecesidad == null) continue;

                var presupMin = itemSeleccionado.PresupuestoMin ?? plantillaNecesidad.PrecioMinOrientativo;
                var presupMax = itemSeleccionado.PresupuestoMax ?? plantillaNecesidad.PrecioMaxOrientativo;

                var necesidad = new NecesidadCrowdsourcing
                {
                    Id = NecesidadCrowdsourcingId.CreateNew(),
                    ProyectoArtisticoId = proyectoArtisticoId,
                    ArtistaId = artistaId,
                    Titulo = plantillaNecesidad.Titulo,
                    Descripcion = plantillaNecesidad.Descripcion,
                    TipoNecesidadId = TipoNecesidadDefault,
                    EstadoNecesidadId = EstadoAbierta,
                    ModalidadTrabajoId = ModalidadRemoto,
                    PresupuestoMin = presupMin,
                    PresupuestoMax = presupMax,
                    MonedaId = itemSeleccionado.MonedaId,
                    FechaCreacion = DateTime.UtcNow
                };

                necesidadesACrear.Add(necesidad);

                presupuestoTotalMin += presupMin ?? 0;
                presupuestoTotalMax += presupMax ?? 0;
                monedaId = itemSeleccionado.MonedaId;
            }

            // 6. Persistir en bulk (transaccion atomica)
            var necesidadIds = await _necesidadService.CreateManyAsync(necesidadesACrear, ct);

            _logger.LogInformation(
                "Generadas {Count} necesidades desde plantilla {PlantillaId} para proyecto {ProyectoId}",
                necesidadIds.Count, request.PlantillaId, request.ProyectoArtisticoId);

            // 7. Retornar resultado
            return new ServiceResponse<GenerarNecesidadesResultDto>
            {
                Data = new GenerarNecesidadesResultDto
                {
                    NecesidadesCreadas = necesidadIds.Count,
                    NecesidadIds = necesidadIds.Select(id => id.Value).ToList(),
                    PresupuestoTotalMin = presupuestoTotalMin,
                    PresupuestoTotalMax = presupuestoTotalMax,
                    Moneda = monedaId
                },
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidades generadas exitosamente a partir de la plantilla",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al generar necesidades desde plantilla {PlantillaId} para proyecto {ProyectoId}",
                request.PlantillaId, request.ProyectoArtisticoId);

            return ValidateExtensions.InternalServerErrorServiceResponse<GenerarNecesidadesResultDto>(
                "Error inesperado al generar necesidades",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
