using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Features.Backings.Commands;
using WePlayRises.Crowdfunding.Application.Features.Backings.Queries;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaniasController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public CampaniasController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<CampaniasController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Get all campaigns with optional filters. Defaults to published campaigns (estadoCampaniaId=2).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [EnableRateLimiting("public-api")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? artistaId,
        [FromQuery] int? estadoCampaniaId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCampaniasQuery
        {
            SearchTerm = searchTerm,
            ArtistaId = artistaId,
            EstadoCampaniaId = estadoCampaniaId ?? 2,
            PageNumber = pageNumber,
            PageSize = Math.Min(pageSize, 50)
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get campaigns of the authenticated artist (includes drafts).
    /// </summary>
    [HttpGet("mis-campanias")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMisCampanias(
        [FromQuery] int? estadoCampaniaId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var artistaId = _currentUser.UserId;
        if (artistaId == null)
        {
            return Unauthorized(new ServiceResponse<IEnumerable<CampaniaListDto>>
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

        var query = new GetMisCampaniasQuery
        {
            ArtistaId = artistaId.Value,
            EstadoCampaniaId = estadoCampaniaId,
            PageNumber = pageNumber,
            PageSize = Math.Min(pageSize, 50)
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get campaign detail with rewards and recent backings (public view).
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [EnableRateLimiting("public-api")]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaDetailDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCampaniaDetailQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Create a new campaign in BORRADOR state. ArtistaId is extracted from JWT token.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCampaniaRequest request,
        CancellationToken cancellationToken)
    {
        var artistaId = _currentUser.UserId;
        if (artistaId == null)
        {
            return Unauthorized(new ServiceResponse<CampaniaDto>
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

        var command = new CreateCampaniaCommand
        {
            ArtistaId = artistaId.Value,
            ProyectoArtisticoId = request.ProyectoArtisticoId,
            Titulo = request.Titulo,
            Subtitulo = request.Subtitulo,
            DescripcionCorta = request.DescripcionCorta,
            VideoPrincipalUrl = request.VideoPrincipalUrl,
            ImagenPrincipalUrl = request.ImagenPrincipalUrl,
            MonedaId = request.MonedaId,
            ImporteObjetivo = request.ImporteObjetivo,
            ImporteMinimo = request.ImporteMinimo,
            TipoFinanciacionId = request.TipoFinanciacionId,
            PermiteAportacionesAnonimas = request.PermiteAportacionesAnonimas,
            PermitePropinas = request.PermitePropinas,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        return FromServiceResponse(result);
    }

    /// <summary>
    /// Update an existing campaign in BORRADOR state. ArtistaId is extracted from JWT token.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCampaniaRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            return BadRequest(new ServiceResponse<bool>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "El Id en URL no coincide con el Id en body",
                        ErrorCode = ServiceResponseMessageType.Validation_InvalidFormat,
                        HttpStatusCode = HttpStatusCode.BadRequest
                    }
                }
            });
        }

        var artistaId = _currentUser.UserId;
        if (artistaId == null)
        {
            return Unauthorized(new ServiceResponse<bool>
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

        var command = new UpdateCampaniaCommand
        {
            Id = request.Id,
            ArtistaId = artistaId.Value,
            Titulo = request.Titulo,
            Subtitulo = request.Subtitulo,
            DescripcionCorta = request.DescripcionCorta,
            VideoPrincipalUrl = request.VideoPrincipalUrl,
            ImagenPrincipalUrl = request.ImagenPrincipalUrl,
            ImporteObjetivo = request.ImporteObjetivo,
            ImporteMinimo = request.ImporteMinimo,
            TipoFinanciacionId = request.TipoFinanciacionId,
            PermiteAportacionesAnonimas = request.PermiteAportacionesAnonimas,
            PermitePropinas = request.PermitePropinas,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin
        };

        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Publish a campaign (transition from BORRADOR to PUBLICADA). ArtistaId is extracted from JWT token.
    /// </summary>
    [HttpPost("{id:guid}/publicar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Publicar(
        Guid id,
        CancellationToken cancellationToken)
    {
        var artistaId = _currentUser.UserId;
        if (artistaId == null)
        {
            return Unauthorized(new ServiceResponse<PublishCampaniaResponse>
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

        var command = new PublishCampaniaCommand
        {
            Id = id,
            ArtistaId = artistaId.Value
        };

        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Delete (soft-delete) a campaign.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCampaniaCommand(id), cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Create a new backing (pledge) for a campaign. Auth is optional.
    /// </summary>
    [HttpPost("{id:guid}/backings")]
    [AllowAnonymous]
    [EnableRateLimiting("public-mutation")]
    [ProducesResponseType(typeof(ServiceResponse<BackingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<BackingDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<BackingDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<BackingDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<BackingDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateBacking(
        Guid id,
        [FromBody] CreateBackingRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBackingCommand
        {
            CampaniaId = id,
            RewardId = request.RewardId,
            Monto = request.Monto,
            Mensaje = request.Mensaje,
            EsAnonimo = request.EsAnonimo,
            UserId = _currentUser.UserId?.ToString(),
            UserName = _currentUser.UserName
        };

        var result = await _mediator.Send(command, cancellationToken);
        return FromServiceResponse(result, HttpStatusCode.Created);
    }

    /// <summary>
    /// Get recent backings for a campaign (public view).
    /// </summary>
    [HttpGet("{id:guid}/backings")]
    [AllowAnonymous]
    [EnableRateLimiting("public-api")]
    [ProducesResponseType(typeof(ServiceResponse<IReadOnlyList<BackingPublicDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<IReadOnlyList<BackingPublicDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<IReadOnlyList<BackingPublicDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBackings(
        Guid id,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBackingsByCampaniaQuery
        {
            CampaniaId = id,
            Limit = Math.Min(limit, 100)
        };

        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get aggregated stats for a campaign's backings.
    /// </summary>
    [HttpGet("{id:guid}/stats")]
    [AllowAnonymous]
    [EnableRateLimiting("public-api")]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<CampaniaStatsDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStats(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCampaniaStatsQuery { CampaniaId = id };
        var result = await _mediator.Send(query, cancellationToken);
        return FromServiceResponse(result);
    }
}
