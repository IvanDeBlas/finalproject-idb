using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CompletarAcuerdoCommand : IRequest<ServiceResponse<CompletarAcuerdoResultDto>>
{
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CompletarAcuerdoCommandHandler : IRequestHandler<CompletarAcuerdoCommand, ServiceResponse<CompletarAcuerdoResultDto>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<CompletarAcuerdoCommand> _validator;
    private readonly ILogger<CompletarAcuerdoCommandHandler> _logger;

    public CompletarAcuerdoCommandHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IArtistaService artistaService,
        IValidator<CompletarAcuerdoCommand> validator,
        ILogger<CompletarAcuerdoCommandHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CompletarAcuerdoResultDto>> Handle(
        CompletarAcuerdoCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<CompletarAcuerdoResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CompletarAcuerdoResultDto>(
                    "Acuerdo no encontrado", ServiceResponseMessageType.NotFound_Acuerdo);
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            if (artista == null || acuerdo.ArtistaId != artista.Id)
            {
                return ValidateExtensions.ForbiddenServiceResponse<CompletarAcuerdoResultDto>(
                    "Solo el artista puede completar el acuerdo", ServiceResponseMessageType.Auth_Forbidden);
            }

            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo)
            {
                return ValidateExtensions.ConflictServiceResponse<CompletarAcuerdoResultDto>(
                    "Solo se pueden completar acuerdos activos", ServiceResponseMessageType.BusinessRule_AcuerdoNotActive);
            }

            await _acuerdoService.CompletarAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);

            var resultDto = new CompletarAcuerdoResultDto
            {
                Id = request.AcuerdoId,
                EstadoAcuerdoNombre = "Completado",
                FechaFinReal = DateTime.UtcNow
            };

            return new ServiceResponse<CompletarAcuerdoResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Acuerdo completado. Puedes dejar una valoracion al profesional."
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing acuerdo {AcuerdoId}", request.AcuerdoId);
            return ValidateExtensions.InternalServerErrorServiceResponse<CompletarAcuerdoResultDto>(
                "Error inesperado al completar acuerdo", ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
