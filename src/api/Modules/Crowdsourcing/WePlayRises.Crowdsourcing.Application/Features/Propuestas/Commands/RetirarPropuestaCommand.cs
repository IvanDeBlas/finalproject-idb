using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class RetirarPropuestaCommand : IRequest<ServiceResponse<RetirarPropuestaResultDto>>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class RetirarPropuestaCommandHandler : IRequestHandler<RetirarPropuestaCommand, ServiceResponse<RetirarPropuestaResultDto>>
{
    private readonly IPropuestaCrowdsourcingService _service;
    private readonly IValidator<RetirarPropuestaCommand> _validator;
    private readonly ILogger<RetirarPropuestaCommandHandler> _logger;

    public RetirarPropuestaCommandHandler(
        IPropuestaCrowdsourcingService service,
        IValidator<RetirarPropuestaCommand> validator,
        ILogger<RetirarPropuestaCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RetirarPropuestaResultDto>> Handle(
        RetirarPropuestaCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<RetirarPropuestaResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var propuestaId = new PropuestaCrowdsourcingId(request.Id);
            var propuesta = await _service.GetByIdAsync(propuestaId, ct);

            if (propuesta == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<RetirarPropuestaResultDto>(
                    "Propuesta no encontrada",
                    ServiceResponseMessageType.NotFound_Propuesta);
            }

            if (propuesta.UserId != request.UserId)
            {
                _logger.LogWarning(
                    "Unauthorized attempt to retirar propuesta {PropuestaId} by user {UserId}. Owner is {OwnerId}",
                    request.Id, request.UserId, propuesta.UserId);

                return ValidateExtensions.ForbiddenServiceResponse<RetirarPropuestaResultDto>(
                    "No tienes permiso para retirar esta propuesta",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            await _service.RetirarAsync(propuestaId, ct);

            var resultDto = new RetirarPropuestaResultDto
            {
                Id = request.Id,
                EstadoPropuestaNombre = "Retirada"
            };

            return new ServiceResponse<RetirarPropuestaResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Propuesta retirada correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retracting propuesta {PropuestaId} by user {UserId}",
                request.Id, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<RetirarPropuestaResultDto>(
                "Error inesperado al retirar propuesta",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
