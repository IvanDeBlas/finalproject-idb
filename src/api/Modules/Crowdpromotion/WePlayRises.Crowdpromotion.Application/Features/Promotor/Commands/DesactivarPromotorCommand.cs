using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;

public class DesactivarPromotorCommand : IRequest<ServiceResponse<PromotorDesactivadoResultDto>>
{
    public string? UserId { get; set; }
}

public class DesactivarPromotorCommandHandler
    : IRequestHandler<DesactivarPromotorCommand, ServiceResponse<PromotorDesactivadoResultDto>>
{
    private readonly IPromotorService _promotorService;
    private readonly ILogger<DesactivarPromotorCommandHandler> _logger;

    public DesactivarPromotorCommandHandler(
        IPromotorService promotorService,
        ILogger<DesactivarPromotorCommandHandler> logger)
    {
        _promotorService = promotorService ?? throw new ArgumentNullException(nameof(promotorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromotorDesactivadoResultDto>> Handle(
        DesactivarPromotorCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return ValidateExtensions.BadRequestServiceResponse<PromotorDesactivadoResultDto>(
                    "Token invalido", ServiceResponseMessageType.Auth_Unauthorized);
            }

            var promotor = await _promotorService.GetByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorDesactivadoResultDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            if (!promotor.EsActivo)
            {
                _logger.LogWarning("Promotor {PromotorId} already inactive", promotor.Id.Value);
                return new ServiceResponse<PromotorDesactivadoResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "El perfil de promotor ya esta desactivado",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_PromotorAlreadyInactive
                        }
                    }
                };
            }

            int programasDadosDeBaja = await _promotorService.DesactivarWithProgramasAsync(promotor.Id, cancellationToken);

            var dto = new PromotorDesactivadoResultDto
            {
                Id = promotor.Id.Value,
                EsActivo = false,
                ProgramasDadosDeBaja = programasDadosDeBaja
            };

            _logger.LogInformation(
                "Promotor {PromotorId} deactivated. Programs deactivated: {Count}",
                promotor.Id.Value, programasDadosDeBaja);

            return new ServiceResponse<PromotorDesactivadoResultDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Perfil desactivado",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating Promotor for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromotorDesactivadoResultDto>(
                "Error inesperado al desactivar el perfil de promotor",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
