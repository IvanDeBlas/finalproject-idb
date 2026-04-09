using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.WebApi.Controllers;

[ApiController]
[Route("api/crowdpromotion/tracking")]
public class TrackingController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public TrackingController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<TrackingController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Registers a tracking event (Click, PageView, Signup, Share).
    /// Public endpoint - no authentication required.
    /// </summary>
    [HttpPost("evento")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrarEvento(
        [FromBody] RegistrarEventoDto request,
        CancellationToken ct)
    {
        var command = new RegistrarEventoCommand
        {
            CodigoReferido = request.CodigoReferido,
            TipoEventoPromoId = request.TipoEventoPromoId,
            CampaniaCrowdfundingId = request.CampaniaCrowdfundingId,
            UrlOrigen = request.UrlOrigen,
            UrlReferer = request.UrlReferer,
            UtmSource = request.UtmSource,
            UtmMedium = request.UtmMedium,
            UtmCampaign = request.UtmCampaign,
            IpOrigen = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserIdAfectado = _currentUser.UserId?.ToString()
        };

        var result = await _mediator.Send(command, ct);

        // Handle 429 rate limit manually (BaseLoggerController maps 4xxx to 409 Conflict)
        if (result.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_RateLimitExcedido))
            return StatusCode(StatusCodes.Status429TooManyRequests, result);

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status201Created, result);

        return FromServiceResponse(result);
    }

    /// <summary>
    /// Registers a conversion (referred backing) with commission calculation.
    /// Internal use - called by the Crowdfunding module handler.
    /// </summary>
    [HttpPost("conversion")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarConversionResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarConversionResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarConversionResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<RegistrarConversionResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrarConversion(
        [FromBody] RegistrarConversionDto request,
        CancellationToken ct)
    {
        var command = new RegistrarConversionCommand
        {
            CodigoReferido = request.CodigoReferido,
            CampaniaCrowdfundingId = request.CampaniaCrowdfundingId,
            AportacionCrowdfundingId = request.AportacionCrowdfundingId,
            ValorMonetario = request.ValorMonetario,
            MonedaId = request.MonedaId,
            UserIdAfectado = request.UserIdAfectado
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status201Created, result);

        return FromServiceResponse(result);
    }
}
