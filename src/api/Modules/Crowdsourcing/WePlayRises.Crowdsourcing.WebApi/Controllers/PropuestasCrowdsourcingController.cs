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
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/propuestas")]
[Authorize]
public class PropuestasCrowdsourcingController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public PropuestasCrowdsourcingController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<PropuestasCrowdsourcingController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet("mis-propuestas")]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<MiPropuestaListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMisPropuestas(
        [FromQuery] int? estado,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var query = new GetMisPropuestasQuery
        {
            UserId = userId,
            EstadoPropuestaId = estado,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpPatch("{id}/retirar")]
    [ProducesResponseType(typeof(ServiceResponse<RetirarPropuestaResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RetirarPropuestaResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Retirar([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new RetirarPropuestaCommand
        {
            Id = id,
            UserId = userId
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }

    [HttpPost("{id}/aceptar")]
    [ProducesResponseType(typeof(ServiceResponse<AceptarPropuestaResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<AceptarPropuestaResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AceptarPropuesta(
        [FromRoute] Guid id,
        [FromBody] AceptarPropuestaCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.PropuestaId = id;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpPatch("{id}/rechazar")]
    [ProducesResponseType(typeof(ServiceResponse<RechazarPropuestaResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RechazarPropuestaResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RechazarPropuesta(
        [FromRoute] Guid id,
        [FromBody] RechazarPropuestaCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.PropuestaId = id;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }
}
