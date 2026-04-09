using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/conversaciones")]
[Authorize]
public class ConversacionesCrowdsourcingController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ConversacionesCrowdsourcingController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<ConversacionesCrowdsourcingController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(ServiceResponse<ConversacionListResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetConversaciones(
        [FromQuery] string contexto = "todas",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var query = new GetConversacionesQuery
        {
            UserId = userId,
            Contexto = contexto,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpGet("no-leidos")]
    [ProducesResponseType(typeof(ServiceResponse<NoLeidosCountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNoLeidos()
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var query = new GetNoLeidosCountQuery { UserId = userId };

        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(ServiceResponse<CreateConversacionResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<CreateConversacionResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateConversacion([FromBody] CreateConversacionCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.UserIdCreador = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpGet("{id:guid}/mensajes")]
    [ProducesResponseType(typeof(ServiceResponse<MensajeListResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMensajes(
        [FromRoute] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var query = new GetMensajesQuery
        {
            ConversacionId = id,
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpPost("{id:guid}/mensajes")]
    [ProducesResponseType(typeof(ServiceResponse<MensajeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<MensajeDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendMensaje(
        [FromRoute] Guid id,
        [FromBody] SendMensajeCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.ConversacionId = id;
        command.UserIdRemitente = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpPatch("{id:guid}/marcar-leidos")]
    [ProducesResponseType(typeof(ServiceResponse<MarcarLeidosResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarcarLeidos([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new MarcarLeidosCommand
        {
            ConversacionId = id,
            UserId = userId
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }
}
