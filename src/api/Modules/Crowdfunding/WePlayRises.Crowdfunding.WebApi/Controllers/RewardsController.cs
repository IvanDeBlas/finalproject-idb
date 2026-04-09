using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RewardsController : BaseLoggerController
{
    private readonly IMediator _mediator;

    public RewardsController(
        IMediator mediator,
        ILogger<RewardsController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all rewards with optional filters.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<RewardListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<RewardListDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? campaniaId,
        [FromQuery] bool? esActivo,
        [FromQuery] bool? esAddOn,
        CancellationToken cancellationToken)
    {
        var query = new GetAllRewardsQuery
        {
            CampaniaId = campaniaId,
            EsActivo = esActivo,
            EsAddOn = esAddOn
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get a reward by its ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRewardByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Create a new reward.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<RewardDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRewardCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Update an existing reward.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRewardCommand command,
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
    /// Delete (deactivate) a reward.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteRewardCommand(id), cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Reorder rewards for a campaign.
    /// </summary>
    [HttpPut("reorder")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Reorder(
        [FromBody] ReorderRewardsCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }
}
