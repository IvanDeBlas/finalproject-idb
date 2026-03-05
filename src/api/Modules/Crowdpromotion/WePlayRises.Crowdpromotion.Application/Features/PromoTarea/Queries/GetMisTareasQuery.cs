using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries;

public class GetMisTareasQuery : IRequest<ServiceResponse<MisTareasResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public string UserId { get; set; } = null!;
}

public class GetMisTareasQueryHandler : IRequestHandler<GetMisTareasQuery, ServiceResponse<MisTareasResponseDto>>
{
    private readonly IPromoTareaService _promoTareaService;
    private readonly ILogger<GetMisTareasQueryHandler> _logger;

    public GetMisTareasQueryHandler(
        IPromoTareaService promoTareaService,
        ILogger<GetMisTareasQueryHandler> logger)
    {
        _promoTareaService = promoTareaService ?? throw new ArgumentNullException(nameof(promoTareaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<MisTareasResponseDto>> Handle(
        GetMisTareasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Resolver Promotor desde UserId
            var promotor = await _promoTareaService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
                return ValidateExtensions.NotFoundServiceResponse<MisTareasResponseDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);

            // 2. Verificar que el programa existe
            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<MisTareasResponseDto>(
                    "El programa de promocion no existe",
                    ServiceResponseMessageType.NotFound_PromoPrograma);

            // 3. Verificar inscripcion aprobada y no bloqueada
            var inscripcion = await _promoTareaService.GetInscripcionAprobadaAsync(
                promotor.Id, programaId, cancellationToken);
            if (inscripcion == null || !inscripcion.EsAprobado || inscripcion.EsBloqueado)
                return ValidateExtensions.ForbiddenServiceResponse<MisTareasResponseDto>(
                    "No tienes acceso a este programa de promocion",
                    ServiceResponseMessageType.Auth_Forbidden);

            // 4. Obtener tareas activas con estado personal
            var dto = await _promoTareaService.GetTareasActivasConEstadoAsync(
                programaId, inscripcion.Id, cancellationToken);

            // 5. Retornar respuesta exitosa
            return new ServiceResponse<MisTareasResponseDto> { Data = dto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mis-tareas for UserId {UserId} ProgramaId {ProgramaId}",
                request.UserId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<MisTareasResponseDto>(
                "Error inesperado al obtener las tareas",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
