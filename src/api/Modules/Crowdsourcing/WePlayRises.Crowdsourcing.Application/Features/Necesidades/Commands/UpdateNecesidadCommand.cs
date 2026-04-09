using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class UpdateNecesidadCommand : IRequest<ServiceResponse<NecesidadUpdateResultDto>>
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
    public Guid ArtistaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class UpdateNecesidadCommandHandler : IRequestHandler<UpdateNecesidadCommand, ServiceResponse<NecesidadUpdateResultDto>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IValidator<UpdateNecesidadCommand> _validator;
    private readonly ILogger<UpdateNecesidadCommandHandler> _logger;

    public UpdateNecesidadCommandHandler(
        INecesidadCrowdsourcingService service,
        IValidator<UpdateNecesidadCommand> validator,
        ILogger<UpdateNecesidadCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<NecesidadUpdateResultDto>> Handle(
        UpdateNecesidadCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<NecesidadUpdateResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var necesidadId = new NecesidadCrowdsourcingId(request.Id);
            var entity = await _service.GetByIdAsync(necesidadId, ct);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<NecesidadUpdateResultDto>(
                    "Necesidad no encontrada",
                    ServiceResponseMessageType.NotFound_Necesidad);
            }

            entity.Titulo = request.Titulo;
            entity.Descripcion = request.Descripcion;
            entity.ModalidadTrabajoId = request.ModalidadTrabajoId;
            entity.PresupuestoMin = request.PresupuestoMin;
            entity.PresupuestoMax = request.PresupuestoMax;
            entity.MonedaId = request.MonedaId;
            entity.UbicacionCiudad = request.UbicacionCiudad;
            entity.UbicacionPais = request.UbicacionPais;
            entity.FechaLimitePropuestas = request.FechaLimitePropuestas;
            entity.FechaInicioPrevista = request.FechaInicioPrevista;
            entity.FechaActualizacion = DateTime.UtcNow;

            await _service.UpdateAsync(entity, ct);

            var resultDto = new NecesidadUpdateResultDto
            {
                Id = entity.Id.Value,
                Titulo = entity.Titulo,
                EstadoNecesidadId = entity.EstadoNecesidadId,
                EstadoNecesidadNombre = "Abierta",
                FechaActualizacion = entity.FechaActualizacion
            };

            return new ServiceResponse<NecesidadUpdateResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidad actualizada correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating NecesidadCrowdsourcing {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadUpdateResultDto>(
                "Error inesperado al actualizar necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
