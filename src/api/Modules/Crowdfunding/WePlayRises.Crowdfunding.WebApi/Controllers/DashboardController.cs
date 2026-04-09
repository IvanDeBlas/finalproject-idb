using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<DashboardController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Get dashboard summary for the authenticated artist.
    /// </summary>
    [HttpGet("resumen")]
    [ProducesResponseType(typeof(ServiceResponse<DashboardResumenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<DashboardResumenDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<DashboardResumenDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<DashboardResumenDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetResumen(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<DashboardResumenDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token invalido",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetDashboardResumenQuery { UserId = userId.Value.ToString() };
        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get paginated list of artist's campaigns with dashboard metrics.
    /// </summary>
    [HttpGet("mis-campanias")]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMisCampanias(
        [FromQuery] int? estadoCampaniaId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token invalido",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetDashboardMisCampaniasQuery
        {
            UserId = userId.Value.ToString(),
            EstadoCampaniaId = estadoCampaniaId,
            Page = page,
            PageSize = Math.Min(pageSize, 100)
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get paginated backings list for a specific campaign (owner only).
    /// </summary>
    [HttpGet("campanias/{campaniaId:guid}/backings")]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaBackingListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaBackingListDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaBackingListDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaBackingListDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaBackingListDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCampaniaBackings(
        Guid campaniaId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<CampaniaBackingListDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token invalido",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetDashboardCampaniaBackingsQuery
        {
            CampaniaId = campaniaId,
            UserId = userId.Value.ToString(),
            Page = page,
            PageSize = Math.Min(pageSize, 100)
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get detailed stats for a specific campaign (owner only).
    /// </summary>
    [HttpGet("campanias/{campaniaId:guid}/stats")]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDetailDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDetailDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCampaniaStats(
        Guid campaniaId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<CampaniaStatsDetailDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token invalido",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetDashboardCampaniaStatsQuery
        {
            CampaniaId = campaniaId,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }
}
