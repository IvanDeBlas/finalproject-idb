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
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing")]
[Authorize]
public class ValoracionesController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ValoracionesController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<ValoracionesController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpPost("acuerdos/{acuerdoId:guid}/valoraciones")]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateValoracion(
        [FromRoute] Guid acuerdoId,
        [FromBody] CreateValoracionCommand command,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.AcuerdoId = acuerdoId;
        command.UserId = userId;

        var response = await _mediator.Send(command, ct);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpGet("usuarios/{userId}/valoraciones")]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionesUsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionesUsuarioDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetValoracionesByUser(
        [FromRoute] string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetValoracionesByUserQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query, ct);
        return FromServiceResponse(response);
    }
}
