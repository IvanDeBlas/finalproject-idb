using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class DeleteCampaniaCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; }

    public DeleteCampaniaCommand(Guid id)
    {
        Id = id;
    }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class DeleteCampaniaCommandHandler : IRequestHandler<DeleteCampaniaCommand, ServiceResponse<bool>>
{
    private readonly ICampaniaService _service;
    private readonly ILogger<DeleteCampaniaCommandHandler> _logger;

    public DeleteCampaniaCommandHandler(
        ICampaniaService service,
        ILogger<DeleteCampaniaCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        DeleteCampaniaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar existencia
            var campaniaId = new CampaniaCrowdfundingId(request.Id);
            var entity = await _service.GetByIdAsync(campaniaId, cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 2. Soft delete (marcar como borrado)
            entity.Borrado = true;
            entity.FechaActualizacion = DateTime.UtcNow;
            await _service.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Campania soft-deleted with Id {Id}", request.Id);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campania eliminada correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Campania with Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al eliminar campania",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
