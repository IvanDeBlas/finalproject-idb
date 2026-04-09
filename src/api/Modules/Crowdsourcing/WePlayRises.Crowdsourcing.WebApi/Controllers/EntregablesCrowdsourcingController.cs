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

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/entregables")]
[Authorize]
public class EntregablesCrowdsourcingController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public EntregablesCrowdsourcingController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<EntregablesCrowdsourcingController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpPatch("{id}/aprobar")]
    [ProducesResponseType(typeof(ServiceResponse<AprobarEntregableResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<AprobarEntregableResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Aprobar(
        [FromRoute] Guid id,
        [FromBody] AprobarEntregableCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.EntregableId = id;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }

    [HttpPatch("{id}/rechazar")]
    [ProducesResponseType(typeof(ServiceResponse<RechazarEntregableResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RechazarEntregableResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Rechazar(
        [FromRoute] Guid id,
        [FromBody] RechazarEntregableCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.EntregableId = id;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }
}
