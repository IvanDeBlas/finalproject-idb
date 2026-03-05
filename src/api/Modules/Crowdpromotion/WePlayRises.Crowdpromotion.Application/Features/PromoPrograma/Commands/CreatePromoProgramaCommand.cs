using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoTareaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoTarea;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;

public class CreatePromoProgramaCommand : IRequest<ServiceResponse<PromoProgramaCreatedResultDto>>
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoPromoId { get; set; }
    public Guid? CampaniaCrowdfundingId { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string? UrlLanding { get; set; }
    public string? CodigoTrackingBase { get; set; }
    public int MonedaId { get; set; }
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public List<CreatePromoTareaItem> Tareas { get; set; } = new();
    public string? UserId { get; set; }

    public class CreatePromoTareaItem
    {
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int TipoEventoPromoId { get; set; }
        public int TipoRewardId { get; set; }
        public decimal? ImporteRecompensa { get; set; }
        public int? MonedaId { get; set; }
        public int? PuntosRecompensa { get; set; }
        public string? UrlInstrucciones { get; set; }
        public bool EsRepetible { get; set; }
        public int? MaxRepeticiones { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
    }
}

public class CreatePromoProgramaCommandHandler
    : IRequestHandler<CreatePromoProgramaCommand, ServiceResponse<PromoProgramaCreatedResultDto>>
{
    private readonly IPromoProgramaService _promoProgramaService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePromoProgramaCommand> _validator;
    private readonly ILogger<CreatePromoProgramaCommandHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromoNombres = new()
    {
        { 1, "Referral" }, { 2, "Afiliado" }, { 3, "Influencer" }, { 4, "Mixto" }
    };

    public CreatePromoProgramaCommandHandler(
        IPromoProgramaService promoProgramaService,
        IMapper mapper,
        IValidator<CreatePromoProgramaCommand> validator,
        ILogger<CreatePromoProgramaCommandHandler> logger)
    {
        _promoProgramaService = promoProgramaService ?? throw new ArgumentNullException(nameof(promoProgramaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromoProgramaCreatedResultDto>> Handle(
        CreatePromoProgramaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreatePromoPrograma: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<PromoProgramaCreatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            if (string.IsNullOrEmpty(request.UserId))
                return ValidateExtensions.BadRequestServiceResponse<PromoProgramaCreatedResultDto>(
                    "Token invalido", ServiceResponseMessageType.Auth_Unauthorized);

            var artistaId = await _promoProgramaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaCreatedResultDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            if (request.CampaniaCrowdfundingId.HasValue)
            {
                var perteneceCapania = await _promoProgramaService.VerificarCampaniaPertenece(
                    artistaId.Value, request.CampaniaCrowdfundingId.Value, cancellationToken);
                if (!perteneceCapania)
                    return ValidateExtensions.ForbiddenServiceResponse<PromoProgramaCreatedResultDto>(
                        "La campana no pertenece a tu perfil de artista", ServiceResponseMessageType.Auth_Forbidden);
            }

            if (request.ProyectoArtisticoId.HasValue)
            {
                var perteneceProyecto = await _promoProgramaService.VerificarProyectoPertenece(
                    artistaId.Value, request.ProyectoArtisticoId.Value, cancellationToken);
                if (!perteneceProyecto)
                    return ValidateExtensions.ForbiddenServiceResponse<PromoProgramaCreatedResultDto>(
                        "El proyecto no pertenece a tu perfil de artista", ServiceResponseMessageType.Auth_Forbidden);
            }

            var programa = _mapper.Map<Domain.Model.PromoPrograma>(request);
            programa.Id = PromoProgramaId.CreateNew();
            programa.ArtistaId = artistaId.Value;
            programa.EsActivo = true;
            programa.FechaCreacion = DateTime.UtcNow;

            var tareas = new List<PromoTareaEntity>();
            foreach (var (tareaItem, index) in request.Tareas.Select((t, i) => (t, i)))
            {
                var tarea = _mapper.Map<PromoTareaEntity>(tareaItem);
                tarea.Id = Guid.NewGuid();
                tarea.EsActivo = true;
                tarea.Orden = index + 1;
                tarea.FechaCreacion = DateTime.UtcNow;
                tareas.Add(tarea);
            }

            var programaId = await _promoProgramaService.CreateWithTareasAsync(programa, tareas, cancellationToken);

            var dto = new PromoProgramaCreatedResultDto
            {
                Id = programaId.Value,
                Titulo = request.Titulo,
                EsActivo = true,
                TareasCreadas = tareas.Count,
                FechaCreacion = programa.FechaCreacion
            };

            if (TipoPromoNombres.TryGetValue(request.TipoPromoId, out var tipoNombre))
                dto.TipoPromoNombre = tipoNombre;

            _logger.LogInformation(
                "PromoPrograma created for ArtistaId {ArtistaId} with Id {ProgramaId}. Tareas: {TareasCount}",
                artistaId.Value, programaId.Value, tareas.Count);

            return new ServiceResponse<PromoProgramaCreatedResultDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Programa de promocion creado", HttpStatusCode = HttpStatusCode.Created }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating PromoPrograma for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromoProgramaCreatedResultDto>(
                "Error inesperado al crear el programa de promocion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
