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

public class UpdatePromoProgramaCommand : IRequest<ServiceResponse<PromoProgramaUpdatedResultDto>>
{
    public Guid Id { get; set; }
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
    public List<UpdatePromoTareaItem> Tareas { get; set; } = new();
    public string? UserId { get; set; }

    public class UpdatePromoTareaItem
    {
        public Guid? Id { get; set; }
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
        public bool EsActivo { get; set; } = true;
    }
}

public class UpdatePromoProgramaCommandHandler
    : IRequestHandler<UpdatePromoProgramaCommand, ServiceResponse<PromoProgramaUpdatedResultDto>>
{
    private readonly IPromoProgramaService _promoProgramaService;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePromoProgramaCommand> _validator;
    private readonly ILogger<UpdatePromoProgramaCommandHandler> _logger;

    public UpdatePromoProgramaCommandHandler(
        IPromoProgramaService promoProgramaService,
        IMapper mapper,
        IValidator<UpdatePromoProgramaCommand> validator,
        ILogger<UpdatePromoProgramaCommandHandler> logger)
    {
        _promoProgramaService = promoProgramaService ?? throw new ArgumentNullException(nameof(promoProgramaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromoProgramaUpdatedResultDto>> Handle(
        UpdatePromoProgramaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdatePromoPrograma: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<PromoProgramaUpdatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            if (string.IsNullOrEmpty(request.UserId))
                return ValidateExtensions.BadRequestServiceResponse<PromoProgramaUpdatedResultDto>(
                    "Token invalido", ServiceResponseMessageType.Auth_Unauthorized);

            var artistaId = await _promoProgramaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaUpdatedResultDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programa = await _promoProgramaService.GetByIdAsync(new PromoProgramaId(request.Id), cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaUpdatedResultDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<PromoProgramaUpdatedResultDto>(
                    "No tienes permiso para modificar este programa", ServiceResponseMessageType.Auth_Forbidden);

            if (request.CampaniaCrowdfundingId.HasValue)
            {
                var pertenece = await _promoProgramaService.VerificarCampaniaPertenece(
                    artistaId.Value, request.CampaniaCrowdfundingId.Value, cancellationToken);
                if (!pertenece)
                    return ValidateExtensions.ForbiddenServiceResponse<PromoProgramaUpdatedResultDto>(
                        "La campana no pertenece a tu perfil de artista", ServiceResponseMessageType.Auth_Forbidden);
            }

            programa.Titulo = request.Titulo;
            programa.Descripcion = request.Descripcion;
            programa.TipoPromoId = request.TipoPromoId;
            programa.CampaniaCrowdfundingId = request.CampaniaCrowdfundingId.HasValue
                ? new CampaniaCrowdfundingId(request.CampaniaCrowdfundingId.Value) : null;
            programa.ProyectoArtisticoId = request.ProyectoArtisticoId.HasValue
                ? new ProyectoArtisticoId(request.ProyectoArtisticoId.Value) : null;
            programa.UrlLanding = request.UrlLanding;
            programa.CodigoTrackingBase = request.CodigoTrackingBase;
            programa.MonedaId = request.MonedaId;
            programa.ImporteComisionPorcentaje = request.ImporteComisionPorcentaje;
            programa.ImporteComisionFija = request.ImporteComisionFija;
            programa.FechaInicio = request.FechaInicio.HasValue
                ? request.FechaInicio.Value.ToDateTime(TimeOnly.MinValue) : null;
            programa.FechaFin = request.FechaFin.HasValue
                ? request.FechaFin.Value.ToDateTime(TimeOnly.MinValue) : null;
            programa.FechaActualizacion = DateTime.UtcNow;

            var tareasNuevas = new List<PromoTareaEntity>();
            var tareasActualizar = new List<PromoTareaEntity>();
            var tareasDesactivar = new List<Guid>();

            foreach (var tareaItem in request.Tareas)
            {
                if (tareaItem.Id == null)
                {
                    var nuevaTarea = _mapper.Map<PromoTareaEntity>(tareaItem);
                    nuevaTarea.Id = Guid.NewGuid();
                    nuevaTarea.ProgramaId = programa.Id;
                    nuevaTarea.EsActivo = true;
                    nuevaTarea.FechaCreacion = DateTime.UtcNow;
                    tareasNuevas.Add(nuevaTarea);
                }
                else if (!tareaItem.EsActivo)
                {
                    tareasDesactivar.Add(tareaItem.Id.Value);
                }
                else
                {
                    var tareaExistente = _mapper.Map<PromoTareaEntity>(tareaItem);
                    tareaExistente.Id = tareaItem.Id.Value;
                    tareaExistente.ProgramaId = programa.Id;
                    tareasActualizar.Add(tareaExistente);
                }
            }

            await _promoProgramaService.UpdateWithTareasAsync(
                programa, tareasNuevas, tareasActualizar, tareasDesactivar, cancellationToken);

            var dto = new PromoProgramaUpdatedResultDto
            {
                Id = programa.Id.Value,
                Titulo = programa.Titulo,
                FechaActualizacion = programa.FechaActualizacion ?? DateTime.UtcNow
            };

            _logger.LogInformation("PromoPrograma {ProgramaId} updated by ArtistaId {ArtistaId}",
                programa.Id.Value, artistaId.Value);

            return new ServiceResponse<PromoProgramaUpdatedResultDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Programa de promocion actualizado", HttpStatusCode = HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating PromoPrograma {ProgramaId} for UserId {UserId}",
                request.Id, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromoProgramaUpdatedResultDto>(
                "Error inesperado al actualizar el programa de promocion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
