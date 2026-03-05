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

public class GetInscripcionesProgramaQuery : IRequest<ServiceResponse<InscripcionListResultDto>>
{
    public Guid ProgramaId { get; set; }
    public string UserId { get; set; } = null!;
    public string? Estado { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetInscripcionesProgramaQueryHandler
    : IRequestHandler<GetInscripcionesProgramaQuery, ServiceResponse<InscripcionListResultDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInscripcionesProgramaQueryHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromotorNombres = new()
    {
        { 1, "Fan Embajador" }, { 2, "Influencer" }, { 3, "Medio / Blog" }, { 4, "Profesional Marketing" }
    };

    public GetInscripcionesProgramaQueryHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        ILogger<GetInscripcionesProgramaQueryHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<InscripcionListResultDto>> Handle(
        GetInscripcionesProgramaQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionListResultDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _inscripcionService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionListResultDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<InscripcionListResultDto>(
                    "No eres propietario de este programa de promocion",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            var (items, totalCount) = await _inscripcionService.GetInscripcionesPorProgramaAsync(
                programaId, request.Estado, request.Page, request.PageSize, cancellationToken);

            var dtoItems = items.Select(inscripcion =>
            {
                var dto = _mapper.Map<InscripcionListItemDto>(inscripcion);

                if (inscripcion.Promotor != null)
                {
                    dto.PromotorId = inscripcion.Promotor.Id.Value;
                    dto.PromotorNombre = inscripcion.Promotor.NombrePublico;
                    dto.PromotorEmailContacto = inscripcion.Promotor.EmailContacto;
                    dto.PromotorUrlInstagram = inscripcion.Promotor.UrlInstagram;
                    dto.PromotorUrlTikTok = inscripcion.Promotor.UrlTikTok;
                    dto.PromotorUrlSitioWeb = inscripcion.Promotor.UrlSitioWeb;

                    if (TipoPromotorNombres.TryGetValue(inscripcion.Promotor.TipoPromotorId, out var tipoNombre))
                        dto.TipoPromotorNombre = tipoNombre;
                }

                dto.Estado = CalcularEstado(inscripcion);
                dto.CodigoReferido = inscripcion.EsAprobado ? inscripcion.CodigoReferido : null;

                return dto;
            }).ToList();

            var totalPages = totalCount > 0
                ? (int)Math.Ceiling((double)totalCount / request.PageSize)
                : 0;

            var resultDto = new InscripcionListResultDto
            {
                Items = dtoItems,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return new ServiceResponse<InscripcionListResultDto> { Data = resultDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inscripciones for ProgramaId {ProgramaId}", request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionListResultDto>(
                "Error inesperado al obtener las inscripciones del programa",
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
