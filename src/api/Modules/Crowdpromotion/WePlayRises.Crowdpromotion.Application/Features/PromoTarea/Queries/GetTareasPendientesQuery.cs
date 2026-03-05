using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries;

public class GetTareasPendientesQuery : IRequest<ServiceResponse<TareasPendientesResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string UserId { get; set; } = null!;
}

public class GetTareasPendientesQueryHandler
    : IRequestHandler<GetTareasPendientesQuery, ServiceResponse<TareasPendientesResponseDto>>
{
    private readonly IPromoTareaService _promoTareaService;
    private readonly ILogger<GetTareasPendientesQueryHandler> _logger;

    public GetTareasPendientesQueryHandler(
        IPromoTareaService promoTareaService,
        ILogger<GetTareasPendientesQueryHandler> logger)
    {
        _promoTareaService = promoTareaService ?? throw new ArgumentNullException(nameof(promoTareaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<TareasPendientesResponseDto>> Handle(
        GetTareasPendientesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Resolver ArtistaId desde UserId
            var artistaId = await _promoTareaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<TareasPendientesResponseDto>(
                    "No tienes un perfil de artista",
                    ServiceResponseMessageType.NotFound_Artista);

            // 2. Verificar que el programa existe
            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<TareasPendientesResponseDto>(
                    "El programa de promocion no existe",
                    ServiceResponseMessageType.NotFound_PromoPrograma);

            // 3. Verificar que el artista es propietario
            if (programa.ArtistaId != artistaId)
                return ValidateExtensions.ForbiddenServiceResponse<TareasPendientesResponseDto>(
                    "No eres el propietario de este programa",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            // 4. Obtener completados pendientes paginados
            var (items, totalCount) = await _promoTareaService.GetTareasPendientesAsync(
                programaId, request.Page, request.PageSize, cancellationToken);

            // 5. Calcular TotalPages y construir DTO
            var totalPages = totalCount > 0
                ? (int)Math.Ceiling((double)totalCount / request.PageSize)
                : 0;

            var dto = new TareasPendientesResponseDto
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return new ServiceResponse<TareasPendientesResponseDto> { Data = dto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tareas-pendientes for UserId {UserId} ProgramaId {ProgramaId}",
                request.UserId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<TareasPendientesResponseDto>(
                "Error inesperado al obtener las tareas pendientes",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
