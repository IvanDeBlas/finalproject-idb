using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : BaseLoggerController
{
    private readonly IMediator _mediator;

    public PedidosController(
        IMediator mediator,
        ILogger<PedidosController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all backings/orders with optional filters.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<PedidoListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<PedidoListDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? campaniaId,
        [FromQuery] string? userId,
        [FromQuery] int? estadoPedidoId,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetAllPedidosQuery
        {
            CampaniaId = campaniaId,
            UserId = userId,
            EstadoPedidoId = estadoPedidoId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get a backing/order by its ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PedidoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PedidoDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PedidoDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPedidoByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Create a new backing/order.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PedidoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PedidoDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PedidoDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PedidoDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePedidoCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Update an existing backing/order.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePedidoCommand command,
        CancellationToken cancellationToken)
    {
        if (!command.Id.Equals(id))
        {
            return BadRequest(new ServiceResponse<bool>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Id en URL no coincide con Id en body",
                        ErrorCode = ServiceResponseMessageType.Validation_InvalidFormat,
                        HttpStatusCode = System.Net.HttpStatusCode.BadRequest
                    }
                }
            });
        }

        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Cancel a backing/order.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePedidoCommand(id), cancellationToken);
        return FromServiceResponse(result);
    }
}
