using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetMisNecesidadesQuery : IRequest<ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>>
{
    public Guid ArtistaId { get; set; }
    public int? EstadoNecesidadId { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetMisNecesidadesQueryHandler : IRequestHandler<GetMisNecesidadesQuery, ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMisNecesidadesQueryHandler> _logger;

    public GetMisNecesidadesQueryHandler(
        INecesidadCrowdsourcingService service,
        IMapper mapper,
        ILogger<GetMisNecesidadesQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>> Handle(
        GetMisNecesidadesQuery request,
        CancellationToken ct)
    {
        try
        {
            var artistaId = new ArtistaId(request.ArtistaId);

            var (items, totalCount) = await _service.GetByArtistaIdPaginatedAsync(
                artistaId,
                request.EstadoNecesidadId,
                request.Search,
                request.Page,
                request.PageSize,
                ct);

            var dtos = _mapper.Map<List<NecesidadCrowdsourcingListDto>>(items);

            var paginatedResponse = new PaginatedResponse<NecesidadCrowdsourcingListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return new ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>
            {
                Data = paginatedResponse,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidades obtenidas exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting necesidades for Artista {ArtistaId}", request.ArtistaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>(
                "Error inesperado al obtener necesidades",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
