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

public class GetMisProgramasQuery : IRequest<ServiceResponse<PromoProgramaListResultDto>>
{
    public string UserId { get; set; } = null!;
    public bool? EsActivo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetMisProgramasQueryHandler
    : IRequestHandler<GetMisProgramasQuery, ServiceResponse<PromoProgramaListResultDto>>
{
    private readonly IPromoProgramaService _promoProgramaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMisProgramasQueryHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromoNombres = new()
    {
        { 1, "Referral" }, { 2, "Afiliado" }, { 3, "Influencer" }, { 4, "Mixto" }
    };

    private static readonly Dictionary<int, string> MonedaNombres = new()
    {
        { 1, "EUR" }, { 2, "USD" }
    };

    public GetMisProgramasQueryHandler(
        IPromoProgramaService promoProgramaService,
        IMapper mapper,
        ILogger<GetMisProgramasQueryHandler> logger)
    {
        _promoProgramaService = promoProgramaService ?? throw new ArgumentNullException(nameof(promoProgramaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromoProgramaListResultDto>> Handle(
        GetMisProgramasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var artistaId = await _promoProgramaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<PromoProgramaListResultDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var (items, totalCount) = await _promoProgramaService.GetMisProgramasAsync(
                artistaId.Value, request.EsActivo, request.Page, request.PageSize, cancellationToken);

            var dtoItems = items.Select(programa =>
            {
                var dto = _mapper.Map<PromoProgramaListItemDto>(programa);

                if (TipoPromoNombres.TryGetValue(programa.TipoPromoId, out var tipoNombre))
                    dto.TipoPromoNombre = tipoNombre;
                if (MonedaNombres.TryGetValue(programa.MonedaId ?? 0, out var monedaNombre))
                    dto.MonedaNombre = monedaNombre;

                dto.NumeroPromotores = programa.Promotores?.Count(p => p.EsActivo) ?? 0;
                dto.NumeroTareas = programa.Tareas?.Count(t => t.EsActivo) ?? 0;

                return dto;
            }).ToList();

            var resultDto = new PromoProgramaListResultDto
            {
                Items = dtoItems,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return new ServiceResponse<PromoProgramaListResultDto> { Data = resultDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MisProgramas for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromoProgramaListResultDto>(
                "Error inesperado al obtener los programas de promocion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
