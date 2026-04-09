using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Pedidos.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetAllPedidosQuery : IRequest<ServiceResponse<IEnumerable<PedidoListDto>>>
{
    public Guid? CampaniaId { get; set; }
    public string? UserId { get; set; }
    public int? EstadoPedidoId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetAllPedidosQueryHandler : IRequestHandler<GetAllPedidosQuery, ServiceResponse<IEnumerable<PedidoListDto>>>
{
    private readonly IPedidoService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllPedidosQueryHandler> _logger;

    public GetAllPedidosQueryHandler(
        IPedidoService service,
        IMapper mapper,
        ILogger<GetAllPedidosQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<IEnumerable<PedidoListDto>>> Handle(
        GetAllPedidosQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyList<Domain.Model.PedidoCrowdfunding> entities;

            // Filtrar por campania si se proporciona
            if (request.CampaniaId.HasValue)
            {
                entities = await _service.GetByCampaniaIdAsync(
                    new CampaniaCrowdfundingId(request.CampaniaId.Value), cancellationToken);
            }
            else if (!string.IsNullOrEmpty(request.UserId))
            {
                entities = await _service.GetByUserIdAsync(request.UserId, cancellationToken);
            }
            else
            {
                entities = await _service.GetAllAsync(cancellationToken);
            }

            // Filtrar por estado
            var filtered = entities.AsEnumerable();

            if (request.EstadoPedidoId.HasValue)
            {
                filtered = filtered.Where(e => e.EstadoPedidoId == request.EstadoPedidoId.Value);
            }

            // Paginacion
            if (request.PageNumber.HasValue && request.PageSize.HasValue)
            {
                filtered = filtered
                    .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value);
            }

            var dtos = _mapper.Map<IEnumerable<PedidoListDto>>(filtered);

            return new ServiceResponse<IEnumerable<PedidoListDto>>
            {
                Data = dtos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all Pedidos");
            return ValidateExtensions.InternalServerErrorServiceResponse<IEnumerable<PedidoListDto>>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
