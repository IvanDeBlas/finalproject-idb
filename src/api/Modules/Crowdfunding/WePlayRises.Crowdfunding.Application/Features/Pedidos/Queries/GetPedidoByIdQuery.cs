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
public class GetPedidoByIdQuery : IRequest<ServiceResponse<PedidoDto>>
{
    public Guid Id { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetPedidoByIdQueryHandler : IRequestHandler<GetPedidoByIdQuery, ServiceResponse<PedidoDto>>
{
    private readonly IPedidoService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPedidoByIdQueryHandler> _logger;

    public GetPedidoByIdQueryHandler(
        IPedidoService service,
        IMapper mapper,
        ILogger<GetPedidoByIdQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PedidoDto>> Handle(
        GetPedidoByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _service.GetByIdAsync(
                new PedidoCrowdfundingId(request.Id), cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PedidoDto>(
                    "Pedido no encontrado",
                    ServiceResponseMessageType.NotFound_Backing);
            }

            var dto = _mapper.Map<PedidoDto>(entity);

            return new ServiceResponse<PedidoDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Pedido encontrado",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Pedido by Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<PedidoDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
