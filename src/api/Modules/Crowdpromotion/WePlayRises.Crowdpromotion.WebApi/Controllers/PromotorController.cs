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
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Queries;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.WebApi.Controllers;

[ApiController]
[Route("api/crowdpromotion/[controller]")]
public class PromotorController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public PromotorController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<PromotorController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Creates the promoter profile for the authenticated user.
    /// One user can only have one promoter profile. UserId is extracted from JWT token.
    /// Also creates a PromotorWallet in EUR automatically.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePromotorRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<PromotorCreatedResultDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var command = new CreatePromotorCommand
        {
            NombrePublico = request.NombrePublico,
            TipoPromotorId = request.TipoPromotorId,
            EmailContacto = request.EmailContacto,
            UrlSitioWeb = request.UrlSitioWeb,
            UrlInstagram = request.UrlInstagram,
            UrlTikTok = request.UrlTikTok,
            UrlYouTube = request.UrlYouTube,
            UrlTwitter = request.UrlTwitter,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetMyProfile), null, result);
        }

        return FromServiceResponse(result);
    }

    /// <summary>
    /// Returns the full promoter profile of the authenticated user, including stats.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<PromotorDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetPromotorMeQuery
        {
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Updates the editable fields of the authenticated user's promoter profile.
    /// TipoPromotorId is NOT editable after creation.
    /// </summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdatePromotorRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<PromotorUpdatedResultDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var command = new UpdatePromotorCommand
        {
            NombrePublico = request.NombrePublico,
            EmailContacto = request.EmailContacto,
            UrlSitioWeb = request.UrlSitioWeb,
            UrlInstagram = request.UrlInstagram,
            UrlTikTok = request.UrlTikTok,
            UrlYouTube = request.UrlYouTube,
            UrlTwitter = request.UrlTwitter,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Logically deactivates the authenticated user's promoter profile.
    /// Sets EsActivo = false and removes the promoter from all active programs.
    /// </summary>
    [HttpPatch("me/desactivar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Desactivar(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<PromotorDesactivadoResultDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var command = new DesactivarPromotorCommand
        {
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Gets the metrics dashboard for the authenticated promoter.
    /// </summary>
    [HttpGet("metricas")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMetricas(
        [FromQuery] Guid? programaId,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<PromotorMetricasResponseDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        DateOnly? parsedDesde = null;
        DateOnly? parsedHasta = null;

        if (!string.IsNullOrEmpty(fechaDesde))
        {
            if (!DateOnly.TryParseExact(fechaDesde, "yyyy-MM-dd", out var d))
                return BadRequest(new ServiceResponse<PromotorMetricasResponseDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Formato de fechaDesde invalido. Use YYYY-MM-DD", ErrorCode = ServiceResponseMessageType.Validation_FechaRangoInvalido }
                    }
                });
            parsedDesde = d;
        }

        if (!string.IsNullOrEmpty(fechaHasta))
        {
            if (!DateOnly.TryParseExact(fechaHasta, "yyyy-MM-dd", out var d))
                return BadRequest(new ServiceResponse<PromotorMetricasResponseDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Formato de fechaHasta invalido. Use YYYY-MM-DD", ErrorCode = ServiceResponseMessageType.Validation_FechaRangoInvalido }
                    }
                });
            parsedHasta = d;
        }

        var query = new GetPromotorMetricasQuery
        {
            UserId = userId.Value.ToString(),
            ProgramaId = programaId,
            FechaDesde = parsedDesde,
            FechaHasta = parsedHasta
        };

        return FromServiceResponse(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// Returns the wallet summary of the authenticated promoter (balance, totals, minimum withdrawal).
    /// </summary>
    [HttpGet("wallet")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromotorWalletDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWallet(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<PromotorWalletDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetPromotorWalletQuery
        {
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Returns the paginated transaction history of the authenticated promoter's wallet.
    /// Supports optional filters by type (credit/debit), status, and date range.
    /// </summary>
    [HttpGet("wallet/transacciones")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<WalletTransaccionesPagedDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWalletTransacciones(
        [FromQuery] bool? esCredito,
        [FromQuery] int? estadoTransaccionId,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<WalletTransaccionesPagedDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        DateTime? parsedDesde = null;
        DateTime? parsedHasta = null;

        if (!string.IsNullOrEmpty(fechaDesde))
        {
            if (!DateTime.TryParse(fechaDesde, out var d))
                return BadRequest(new ServiceResponse<WalletTransaccionesPagedDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Formato de fechaDesde invalido", ErrorCode = ServiceResponseMessageType.Validation_FechaRangoInvalido }
                    }
                });
            parsedDesde = d;
        }

        if (!string.IsNullOrEmpty(fechaHasta))
        {
            if (!DateTime.TryParse(fechaHasta, out var d))
                return BadRequest(new ServiceResponse<WalletTransaccionesPagedDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Formato de fechaHasta invalido", ErrorCode = ServiceResponseMessageType.Validation_FechaRangoInvalido }
                    }
                });
            parsedHasta = d;
        }

        var query = new GetWalletTransaccionesQuery
        {
            UserId = userId.Value.ToString(),
            EsCredito = esCredito,
            EstadoTransaccionId = estadoTransaccionId,
            FechaDesde = parsedDesde,
            FechaHasta = parsedHasta,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Submits a withdrawal request from the authenticated promoter's wallet.
    /// Creates a pending debit transaction and decreases SaldoDisponible with optimistic concurrency.
    /// </summary>
    [HttpPost("wallet/cobro")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<SolicitarCobroResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SolicitarCobro(
        [FromBody] SolicitarCobroRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<SolicitarCobroResponseDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token no valido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var command = new SolicitarCobroCommand
        {
            UserId = userId.Value.ToString(),
            Importe = request.Importe,
            Descripcion = request.Descripcion
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetWallet), null, result);
        }

        return FromServiceResponse(result);
    }
}
