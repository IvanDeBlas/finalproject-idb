using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class UpdateRewardCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int? TipoRewardId { get; set; }
    public int? MonedaId { get; set; }
    public bool? EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; }
    public int? CantidadPorBacker { get; set; }
    public bool? IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int? Orden { get; set; }
    public bool? EsActivo { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class UpdateRewardCommandHandler : IRequestHandler<UpdateRewardCommand, ServiceResponse<bool>>
{
    private readonly IRewardService _service;
    private readonly IValidator<UpdateRewardCommand> _validator;
    private readonly ILogger<UpdateRewardCommandHandler> _logger;

    public UpdateRewardCommandHandler(
        IRewardService service,
        IValidator<UpdateRewardCommand> validator,
        ILogger<UpdateRewardCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        UpdateRewardCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateReward: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<bool>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Obtener entidad existente
            var rewardId = new CampaniaCrowdfundingRewardId(request.Id);
            var entity = await _service.GetByIdAsync(rewardId, cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Reward no encontrado",
                    ServiceResponseMessageType.NotFound_Reward);
            }

            // 3. Validar que reward NO tiene backings antes de modificar campos criticos
            var hasBackings = await _service.HasBackingsAsync(rewardId, cancellationToken);
            if (hasBackings)
            {
                if (request.Nombre != null || request.ImporteMinimo.HasValue ||
                    request.CantidadMaxima.HasValue || request.TipoRewardId.HasValue ||
                    request.MonedaId.HasValue || request.EsAddOn.HasValue ||
                    request.CantidadPorBacker.HasValue || request.IncluyeEnvioFisico.HasValue ||
                    request.Orden.HasValue)
                {
                    return ValidateExtensions.ConflictServiceResponse<bool>(
                        "No se puede modificar campos criticos de una recompensa con aportes existentes. Solo puedes editar Descripcion, Tiempo de Entrega y Estado.",
                        ServiceResponseMessageType.BusinessRule_RewardHasBackings);
                }
            }

            // 4. Actualizar solo propiedades proporcionadas
            if (!string.IsNullOrEmpty(request.Nombre))
                entity.Nombre = request.Nombre;
            if (request.Descripcion != null)
                entity.Descripcion = request.Descripcion;
            if (request.ImporteMinimo.HasValue)
                entity.ImporteMinimo = request.ImporteMinimo.Value;
            if (request.TipoRewardId.HasValue)
                entity.TipoRewardId = request.TipoRewardId.Value;
            if (request.MonedaId.HasValue)
                entity.MonedaId = request.MonedaId.Value;
            if (request.EsAddOn.HasValue)
                entity.EsAddOn = request.EsAddOn.Value;
            if (request.CantidadMaxima.HasValue)
                entity.CantidadMaxima = request.CantidadMaxima.Value;
            if (request.CantidadPorBacker.HasValue)
                entity.CantidadPorBacker = request.CantidadPorBacker.Value;
            if (request.IncluyeEnvioFisico.HasValue)
                entity.IncluyeEnvioFisico = request.IncluyeEnvioFisico.Value;
            if (request.TiempoEntregaEstimado != null)
                entity.TiempoEntregaEstimado = request.TiempoEntregaEstimado;
            if (request.Orden.HasValue)
                entity.Orden = request.Orden.Value;
            if (request.EsActivo.HasValue)
                entity.EsActivo = request.EsActivo.Value;

            entity.FechaActualizacion = DateTime.UtcNow;

            // 4. Actualizar via servicio
            await _service.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Reward updated with Id {Id}", request.Id);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Reward actualizado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Reward with Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al actualizar reward",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
