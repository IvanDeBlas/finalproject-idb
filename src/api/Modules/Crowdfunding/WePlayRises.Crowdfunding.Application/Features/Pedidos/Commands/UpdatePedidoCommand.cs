using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class UpdatePedidoCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; set; }
    public int? EstadoPedidoId { get; set; }
    public decimal? ImportePropina { get; set; }
    public bool? PermitirMostrarNombre { get; set; }
    public string? ComentarioBacker { get; set; }
    public Guid? DireccionEnvioId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class UpdatePedidoCommandHandler : IRequestHandler<UpdatePedidoCommand, ServiceResponse<bool>>
{
    private readonly IPedidoService _service;
    private readonly IValidator<UpdatePedidoCommand> _validator;
    private readonly ILogger<UpdatePedidoCommandHandler> _logger;

    public UpdatePedidoCommandHandler(
        IPedidoService service,
        IValidator<UpdatePedidoCommand> validator,
        ILogger<UpdatePedidoCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        UpdatePedidoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdatePedido: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<bool>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Obtener entidad existente
            var pedidoId = new PedidoCrowdfundingId(request.Id);
            var entity = await _service.GetByIdAsync(pedidoId, cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Pedido no encontrado",
                    ServiceResponseMessageType.NotFound_Backing);
            }

            // 3. Actualizar solo propiedades proporcionadas
            if (request.EstadoPedidoId.HasValue)
                entity.EstadoPedidoId = request.EstadoPedidoId.Value;
            if (request.ImportePropina.HasValue)
                entity.ImportePropina = request.ImportePropina.Value;
            if (request.PermitirMostrarNombre.HasValue)
                entity.PermitirMostrarNombre = request.PermitirMostrarNombre.Value;
            if (request.ComentarioBacker != null)
                entity.ComentarioBacker = request.ComentarioBacker;
            if (request.DireccionEnvioId.HasValue)
                entity.DireccionEnvioId = request.DireccionEnvioId.Value;

            entity.FechaActualizacion = DateTime.UtcNow;

            // 4. Actualizar via servicio
            await _service.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Pedido updated with Id {Id}", request.Id);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Pedido actualizado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Pedido with Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al actualizar pedido",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
