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
public class DeletePedidoCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; }

    public DeletePedidoCommand(Guid id)
    {
        Id = id;
    }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class DeletePedidoCommandHandler : IRequestHandler<DeletePedidoCommand, ServiceResponse<bool>>
{
    private readonly IPedidoService _service;
    private readonly ILogger<DeletePedidoCommandHandler> _logger;

    public DeletePedidoCommandHandler(
        IPedidoService service,
        ILogger<DeletePedidoCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        DeletePedidoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar existencia
            var pedidoId = new PedidoCrowdfundingId(request.Id);
            var entity = await _service.GetByIdAsync(pedidoId, cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Pedido no encontrado",
                    ServiceResponseMessageType.NotFound_Backing);
            }

            // 2. Cancelar pedido (cambiar estado, no eliminar fisicamente)
            entity.EstadoPedidoId = 5; // Cancelado
            entity.FechaActualizacion = DateTime.UtcNow;
            await _service.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Pedido cancelled with Id {Id}", request.Id);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Pedido cancelado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Pedido with Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al cancelar pedido",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
