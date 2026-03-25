using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetAllCampaniasQuery : IRequest<ServiceResponse<IEnumerable<CampaniaListDto>>>
{
    public string? SearchTerm { get; set; }
    public Guid? ArtistaId { get; set; }
    public int? EstadoCampaniaId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetAllCampaniasQueryHandler : IRequestHandler<GetAllCampaniasQuery, ServiceResponse<IEnumerable<CampaniaListDto>>>
{
    private readonly ICampaniaService _service;
    private readonly ICrowdFlagsService _crowdFlagsService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllCampaniasQueryHandler> _logger;

    public GetAllCampaniasQueryHandler(
        ICampaniaService service,
        ICrowdFlagsService crowdFlagsService,
        IMapper mapper,
        ILogger<GetAllCampaniasQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _crowdFlagsService = crowdFlagsService ?? throw new ArgumentNullException(nameof(crowdFlagsService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<IEnumerable<CampaniaListDto>>> Handle(
        GetAllCampaniasQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyList<Domain.Model.CampaniaCrowdfunding> entities;

            // Filtrar por artista si se proporciona
            if (request.ArtistaId.HasValue)
            {
                entities = await _service.GetByArtistaIdAsync(
                    new ArtistaId(request.ArtistaId.Value), cancellationToken);
            }
            else
            {
                entities = await _service.GetAllAsync(cancellationToken);
            }

            // Filtrar por estado
            var filtered = entities.AsEnumerable();

            if (request.EstadoCampaniaId.HasValue)
            {
                filtered = filtered.Where(e => e.EstadoCampaniaId == request.EstadoCampaniaId.Value);
            }

            // Filtrar por termino de busqueda
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filtered = filtered.Where(e =>
                    e.Titulo.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (e.DescripcionCorta != null && e.DescripcionCorta.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            // Excluir borrados
            filtered = filtered.Where(e => !e.Borrado);

            // Paginacion
            if (request.PageNumber.HasValue && request.PageSize.HasValue)
            {
                filtered = filtered
                    .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value);
            }

            var dtos = _mapper.Map<IEnumerable<CampaniaListDto>>(filtered);

            // Enrich with crowd flags
            var dtoList = dtos.ToList();
            var proyectoIds = dtoList
                .Where(d => d.ProyectoArtisticoId.HasValue)
                .Select(d => d.ProyectoArtisticoId!.Value)
                .Distinct()
                .ToList();

            if (proyectoIds.Count > 0)
            {
                var flags = await _crowdFlagsService.GetFlagsForProyectosAsync(proyectoIds, cancellationToken);
                foreach (var dto in dtoList.Where(d => d.ProyectoArtisticoId.HasValue))
                {
                    if (flags.TryGetValue(dto.ProyectoArtisticoId!.Value, out var f))
                    {
                        dto.TieneCrowdsourcing = f.TieneCrowdsourcing;
                        dto.TieneCrowdpromotion = f.TieneCrowdpromotion;
                    }
                }
            }

            return new ServiceResponse<IEnumerable<CampaniaListDto>>
            {
                Data = dtoList
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all Campanias");
            return ValidateExtensions.InternalServerErrorServiceResponse<IEnumerable<CampaniaListDto>>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
