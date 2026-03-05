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
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Queries;

public class ExplorarProgramasQuery : IRequest<ServiceResponse<ExplorarProgramasResultDto>>
{
    public string UserId { get; set; } = null!;
    public string? ArtistaNombre { get; set; }
    public int? TipoPromoId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ExplorarProgramasQueryHandler
    : IRequestHandler<ExplorarProgramasQuery, ServiceResponse<ExplorarProgramasResultDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly ILogger<ExplorarProgramasQueryHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromoNombres = new()
    {
        { 1, "Referral" }, { 2, "Afiliado" }, { 3, "Influencer" }, { 4, "Mixto" }
    };

    private static readonly Dictionary<int, string> MonedaNombres = new()
    {
        { 1, "EUR" }, { 2, "USD" }
    };

    public ExplorarProgramasQueryHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        ILogger<ExplorarProgramasQueryHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ExplorarProgramasResultDto>> Handle(
        ExplorarProgramasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var promotor = await _inscripcionService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
                return ValidateExtensions.NotFoundServiceResponse<ExplorarProgramasResultDto>(
                    "No tienes un perfil de promotor", ServiceResponseMessageType.NotFound_Promotor);

            var (items, totalCount) = await _inscripcionService.GetProgramasActivosAsync(
                request.ArtistaNombre, request.TipoPromoId, request.Page, request.PageSize,
                promotor.Id, cancellationToken);

            var dtoItems = items.Select(programa =>
            {
                var dto = _mapper.Map<ProgramaExplorarItemDto>(programa);

                if (TipoPromoNombres.TryGetValue(programa.TipoPromoId, out var tipoNombre))
                    dto.TipoPromoNombre = tipoNombre;
                if (MonedaNombres.TryGetValue(programa.MonedaId ?? 0, out var monedaNombre))
                    dto.MonedaNombre = monedaNombre;

                dto.NumeroTareas = programa.Tareas?.Count(t => t.EsActivo) ?? 0;

                var miInscripcion = programa.Promotores?.FirstOrDefault();
                dto.MiEstado = miInscripcion == null ? null : CalcularEstado(miInscripcion);

                // MVP: cross-context query TBD
                dto.ArtistaNombre = string.Empty;

                return dto;
            }).ToList();

            var totalPages = totalCount > 0
                ? (int)Math.Ceiling((double)totalCount / request.PageSize)
                : 0;

            var resultDto = new ExplorarProgramasResultDto
            {
                Items = dtoItems,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return new ServiceResponse<ExplorarProgramasResultDto> { Data = resultDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exploring programas for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ExplorarProgramasResultDto>(
                "Error inesperado al explorar programas de promocion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }

    private static string CalcularEstado(PromoProgramaPromotor inscripcion)
    {
        if (inscripcion.EsBloqueado) return "Bloqueado";
        if (inscripcion.FechaBaja != null) return "DadoDeBaja";
        if (inscripcion.EsAprobado) return "Aprobado";
        return "Pendiente";
    }
}
