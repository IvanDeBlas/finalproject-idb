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
public class CerrarNecesidadCommand : IRequest<ServiceResponse<CerrarNecesidadResultDto>>
{
    public Guid Id { get; set; }
    public string? Motivo { get; set; }
    public Guid ArtistaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CerrarNecesidadCommandHandler : IRequestHandler<CerrarNecesidadCommand, ServiceResponse<CerrarNecesidadResultDto>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IValidator<CerrarNecesidadCommand> _validator;
    private readonly ILogger<CerrarNecesidadCommandHandler> _logger;

    public CerrarNecesidadCommandHandler(
        INecesidadCrowdsourcingService service,
        IValidator<CerrarNecesidadCommand> validator,
        ILogger<CerrarNecesidadCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CerrarNecesidadResultDto>> Handle(
        CerrarNecesidadCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<CerrarNecesidadResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var necesidadId = new NecesidadCrowdsourcingId(request.Id);
            var propuestasRechazadas = await _service.CerrarAsync(necesidadId, request.Motivo, ct);

            var resultDto = new CerrarNecesidadResultDto
            {
                Id = request.Id,
                EstadoNecesidadNombre = "Cerrada",
                PropuestasRechazadas = propuestasRechazadas
            };

            var mensaje = propuestasRechazadas > 0
                ? $"Necesidad cerrada correctamente. Se han rechazado {propuestasRechazadas} propuestas pendientes."
                : "Necesidad cerrada correctamente.";

            return new ServiceResponse<CerrarNecesidadResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = mensaje,
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing NecesidadCrowdsourcing {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<CerrarNecesidadResultDto>(
                "Error inesperado al cerrar necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
