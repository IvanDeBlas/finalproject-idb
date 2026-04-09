using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Queries;

public class GetPromoProgramaByIdQuery : IRequest<ServiceResponse<PromoProgramaDetailDto>>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
}

public class GetPromoProgramaByIdQueryHandler
    : IRequestHandler<GetPromoProgramaByIdQuery, ServiceResponse<PromoProgramaDetailDto>>
{
    private readonly IPromoProgramaService _promoProgramaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPromoProgramaByIdQueryHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromoNombres = new()
    {
        { 1, "Referral" }, { 2, "Afiliado" }, { 3, "Influencer" }, { 4, "Mixto" }
    };

    private static readonly Dictionary<int, string> TipoEventoPromoNombres = new()
    {
        { 1, "Click" }, { 2, "PageView" }, { 3, "Share" }, { 4, "Post" }, { 5, "Signup" }, { 6, "Backing" }
    };

    private static readonly Dictionary<int, string> TipoRewardNombres = new()
    {
        { 1, "Dinero" }, { 2, "Puntos" }, { 3, "Mixto" }
    };

    private static readonly Dictionary<int, string> MonedaNombres = new()
    {
        { 1, "EUR" }, { 2, "USD" }
    };

    private static readonly Dictionary<int, string> TipoPromotorNombres = new()
    {
        { 1, "Fan Embajador" }, { 2, "Influencer" }, { 3, "Medio / Blog" }, { 4, "Profesional Marketing" }
    };

    public GetPromoProgramaByIdQueryHandler(
        IPromoProgramaService promoProgramaService,
        IMapper mapper,
        ILogger<GetPromoProgramaByIdQueryHandler> logger)
    {
        _promoProgramaService = promoProgramaService ?? throw new ArgumentNullException(nameof(promoProgramaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromoProgramaDetailDto>> Handle(
        GetPromoProgramaByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var artistaId = await _promoProgramaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaDetailDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programa = await _promoProgramaService.GetByIdWithFullDetailAsync(
                new PromoProgramaId(request.Id), cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaDetailDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<PromoProgramaDetailDto>(
                    "No tienes permiso para ver este programa", ServiceResponseMessageType.Auth_Forbidden);

            var dto = _mapper.Map<PromoProgramaDetailDto>(programa);

            if (TipoPromoNombres.TryGetValue(programa.TipoPromoId, out var tipoNombre))
                dto.TipoPromoNombre = tipoNombre;
            if (MonedaNombres.TryGetValue(programa.MonedaId ?? 0, out var monedaNombre))
                dto.MonedaNombre = monedaNombre;

            dto.Tareas = programa.Tareas?.Select(tarea =>
            {
                var tareaDto = _mapper.Map<PromoTareaDetailDto>(tarea);
                if (TipoEventoPromoNombres.TryGetValue(tarea.TipoEventoPromoId, out var eventoNombre))
                    tareaDto.TipoEventoPromoNombre = eventoNombre;
                if (TipoRewardNombres.TryGetValue(tarea.TipoRewardId ?? 0, out var rewardNombre))
                    tareaDto.TipoRewardNombre = rewardNombre;
                if (tarea.MonedaId.HasValue && MonedaNombres.TryGetValue(tarea.MonedaId.Value, out var mNombre))
                    tareaDto.MonedaNombre = mNombre;
                tareaDto.CompletadosPorPromotores = tarea.TareasPromotor?.Count ?? 0;
                return tareaDto;
            }).ToList() ?? new List<PromoTareaDetailDto>();

            dto.Promotores = programa.Promotores?.Select(pp =>
            {
                var ppDto = new PromoProgramaPromotorSummaryDto
                {
                    Id = pp.Id,
                    PromotorNombre = pp.Promotor?.NombrePublico ?? string.Empty,
                    EsAprobado = pp.EsActivo,
                    EsBloqueado = !pp.EsActivo && pp.FechaBaja.HasValue,
                    FechaAlta = pp.FechaInscripcion
                };
                if (pp.Promotor != null && TipoPromotorNombres.TryGetValue(pp.Promotor.TipoPromotorId, out var tipoPromNombre))
                    ppDto.TipoPromotorNombre = tipoPromNombre;
                return ppDto;
            }).ToList() ?? new List<PromoProgramaPromotorSummaryDto>();

            dto.Resumen = await _promoProgramaService.GetResumenAsync(
                new PromoProgramaId(programa.Id.Value), cancellationToken);

            return new ServiceResponse<PromoProgramaDetailDto> { Data = dto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting PromoPrograma {ProgramaId} for UserId {UserId}",
                request.Id, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromoProgramaDetailDto>(
                "Error inesperado al obtener el programa de promocion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
