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

public class GetMisInscripcionesQuery : IRequest<ServiceResponse<MisInscripcionesResultDto>>
{
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetMisInscripcionesQueryHandler
    : IRequestHandler<GetMisInscripcionesQuery, ServiceResponse<MisInscripcionesResultDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMisInscripcionesQueryHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromoNombres = new()
    {
        { 1, "Referral" }, { 2, "Afiliado" }, { 3, "Influencer" }, { 4, "Mixto" }
    };

    private static readonly Dictionary<int, string> MonedaNombres = new()
    {
        { 1, "EUR" }, { 2, "USD" }
    };

    private static readonly Dictionary<int, string> TipoEventoPromoNombres = new()
    {
        { 1, "Click" }, { 2, "PageView" }, { 3, "Share" }, { 4, "Post" }, { 5, "Signup" }, { 6, "Backing" }
    };

    public GetMisInscripcionesQueryHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        ILogger<GetMisInscripcionesQueryHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<MisInscripcionesResultDto>> Handle(
        GetMisInscripcionesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var promotor = await _inscripcionService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
                return ValidateExtensions.NotFoundServiceResponse<MisInscripcionesResultDto>(
                    "No tienes un perfil de promotor", ServiceResponseMessageType.NotFound_Promotor);

            var (items, totalCount) = await _inscripcionService.GetMisInscripcionesAsync(
                promotor.Id, request.Page, request.PageSize, cancellationToken);

            var dtoItems = items.Select(inscripcion =>
            {
                var dto = _mapper.Map<MiInscripcionDto>(inscripcion);

                if (inscripcion.Programa != null)
                {
                    dto.ProgramaTitulo = inscripcion.Programa.Titulo;

                    if (TipoPromoNombres.TryGetValue(inscripcion.Programa.TipoPromoId, out var tipoNombre))
                        dto.TipoPromoNombre = tipoNombre;

                    dto.ImporteComisionPorcentaje = inscripcion.Programa.ImporteComisionPorcentaje;
                    dto.ImporteComisionFija = inscripcion.Programa.ImporteComisionFija;

                    if (MonedaNombres.TryGetValue(inscripcion.Programa.MonedaId ?? 0, out var monedaNombre))
                        dto.MonedaNombre = monedaNombre;
                }

                dto.Estado = CalcularEstado(inscripcion);

                // MVP: cross-context query TBD
                dto.ArtistaNombre = string.Empty;

                if (inscripcion.EsAprobado && inscripcion.FechaBaja == null)
                {
                    dto.CodigoReferido = inscripcion.CodigoReferido;
                    dto.UrlTrackingPersonalizada = inscripcion.UrlReferido;

                    if (inscripcion.Programa?.Tareas != null)
                    {
                        dto.Tareas = inscripcion.Programa.Tareas
                            .Where(t => t.EsActivo)
                            .OrderBy(t => t.Orden)
                            .Select(t =>
                            {
                                var tareaDto = _mapper.Map<TareaResumenDto>(t);

                                if (TipoEventoPromoNombres.TryGetValue(t.TipoEventoPromoId, out var eventoNombre))
                                    tareaDto.TipoEventoPromoNombre = eventoNombre;
                                if (MonedaNombres.TryGetValue(t.MonedaId ?? 0, out var tareaMonedaNombre))
                                    tareaDto.MonedaNombre = tareaMonedaNombre;

                                return tareaDto;
                            })
                            .ToList();
                    }
                    else
                    {
                        dto.Tareas = new List<TareaResumenDto>();
                    }
                }
                else
                {
                    dto.CodigoReferido = null;
                    dto.UrlTrackingPersonalizada = null;
                    dto.Tareas = new List<TareaResumenDto>();
                }

                return dto;
            }).ToList();

            var totalPages = totalCount > 0
                ? (int)Math.Ceiling((double)totalCount / request.PageSize)
                : 0;

            var resultDto = new MisInscripcionesResultDto
            {
                Items = dtoItems,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return new ServiceResponse<MisInscripcionesResultDto> { Data = resultDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mis inscripciones for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<MisInscripcionesResultDto>(
                "Error inesperado al obtener tus inscripciones",
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
