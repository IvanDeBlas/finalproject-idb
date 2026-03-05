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
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/acuerdos")]
[Authorize]
public class AcuerdosCrowdsourcingController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AcuerdosCrowdsourcingController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<AcuerdosCrowdsourcingController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<AcuerdoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var query = new GetAcuerdoByIdQuery
        {
            AcuerdoId = id,
            UserId = userId
        };

        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpPost("{acuerdoId}/milestones")]
    [ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMilestone(
        [FromRoute] Guid acuerdoId,
        [FromBody] CreateMilestoneCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.AcuerdoId = acuerdoId;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpPut("{acuerdoId}/milestones/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateMilestone(
        [FromRoute] Guid acuerdoId,
        [FromRoute] Guid id,
        [FromBody] UpdateMilestoneCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.AcuerdoId = acuerdoId;
        command.MilestoneId = id;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }

    [HttpDelete("{acuerdoId}/milestones/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMilestone(
        [FromRoute] Guid acuerdoId,
        [FromRoute] Guid id)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new DeleteMilestoneCommand
        {
            AcuerdoId = acuerdoId,
            MilestoneId = id,
            UserId = userId
        };

        var response = await _mediator.Send(command);

        if (response.IsSuccess)
            return NoContent();

        return FromServiceResponse(response);
    }

    [HttpPost("{acuerdoId}/entregables")]
    [ProducesResponseType(typeof(ServiceResponse<EntregableCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<EntregableCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEntregable(
        [FromRoute] Guid acuerdoId,
        [FromBody] CreateEntregableCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.AcuerdoId = acuerdoId;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpPatch("{id}/completar")]
    [ProducesResponseType(typeof(ServiceResponse<CompletarAcuerdoResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<CompletarAcuerdoResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Completar([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new CompletarAcuerdoCommand
        {
            AcuerdoId = id,
            UserId = userId
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }

    [HttpPatch("{id}/cancelar")]
    [ProducesResponseType(typeof(ServiceResponse<CancelarAcuerdoResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<CancelarAcuerdoResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancelar(
        [FromRoute] Guid id,
        [FromBody] CancelarAcuerdoCommand command)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.AcuerdoId = id;
        command.UserId = userId;

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }
}
