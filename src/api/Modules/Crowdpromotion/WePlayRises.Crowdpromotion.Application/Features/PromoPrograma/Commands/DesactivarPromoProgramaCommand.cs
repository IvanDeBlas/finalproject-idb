using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;

public class DesactivarPromoProgramaCommand : IRequest<ServiceResponse<PromoProgramaDesactivadoResultDto>>
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
}

public class DesactivarPromoProgramaCommandHandler
    : IRequestHandler<DesactivarPromoProgramaCommand, ServiceResponse<PromoProgramaDesactivadoResultDto>>
{
    private readonly IPromoProgramaService _promoProgramaService;
    private readonly ILogger<DesactivarPromoProgramaCommandHandler> _logger;

    public DesactivarPromoProgramaCommandHandler(
        IPromoProgramaService promoProgramaService,
        ILogger<DesactivarPromoProgramaCommandHandler> logger)
    {
        _promoProgramaService = promoProgramaService ?? throw new ArgumentNullException(nameof(promoProgramaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromoProgramaDesactivadoResultDto>> Handle(
        DesactivarPromoProgramaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId))
                return ValidateExtensions.BadRequestServiceResponse<PromoProgramaDesactivadoResultDto>(
                    "Token invalido", ServiceResponseMessageType.Auth_Unauthorized);

            var artistaId = await _promoProgramaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaDesactivadoResultDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programa = await _promoProgramaService.GetByIdAsync(new PromoProgramaId(request.Id), cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaDesactivadoResultDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<PromoProgramaDesactivadoResultDto>(
                    "No tienes permiso para desactivar este programa", ServiceResponseMessageType.Auth_Forbidden);

            if (!programa.EsActivo)
            {
                _logger.LogWarning("PromoPrograma {ProgramaId} already inactive", programa.Id.Value);
                return new ServiceResponse<PromoProgramaDesactivadoResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "El programa de promocion ya esta desactivado",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_PromoProgramaAlreadyInactive
                        }
                    }
                };
            }

            var tareasDesactivadas = await _promoProgramaService.DesactivarWithTareasAsync(
                new PromoProgramaId(programa.Id.Value), cancellationToken);

            var dto = new PromoProgramaDesactivadoResultDto
            {
                Id = programa.Id.Value,
                EsActivo = false,
                TareasDesactivadas = tareasDesactivadas
            };

            _logger.LogInformation(
                "PromoPrograma {ProgramaId} deactivated. Tareas deactivated: {Count}",
                programa.Id.Value, tareasDesactivadas);

            return new ServiceResponse<PromoProgramaDesactivadoResultDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Programa desactivado", HttpStatusCode = HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating PromoPrograma {ProgramaId} for UserId {UserId}",
                request.Id, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromoProgramaDesactivadoResultDto>(
                "Error inesperado al desactivar el programa de promocion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
