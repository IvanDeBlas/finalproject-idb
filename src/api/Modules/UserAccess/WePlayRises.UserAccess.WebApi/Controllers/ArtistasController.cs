using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artistas.Commands;
using WePlayRises.UserAccess.Application.Features.Artistas.Queries;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistasController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ArtistasController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<ArtistasController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Creates a new artist profile linked to the authenticated user.
    /// </summary>
    /// <param name="command">Profile data (nombreArtistico, descripcion, etc.)</param>
    /// <returns>Created artist profile</returns>
    /// <response code="200">Profile created successfully</response>
    /// <response code="400">Validation errors</response>
    /// <response code="401">Invalid or expired JWT token</response>
    /// <response code="409">User already has an artist profile</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateArtistaCommand command)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<ArtistaDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token invalido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        command.UserId = userId.Value.ToString();
        var result = await _mediator.Send(command);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Gets the artist profile of the currently authenticated user.
    /// </summary>
    /// <returns>Artist profile or 404 if no profile exists</returns>
    /// <response code="200">Artist found</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">No artist profile for this user</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<ArtistaDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token invalido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var userIdStr = userId.Value.ToString();
        var query = new GetArtistaByUserIdQuery
        {
            UserId = userIdStr,
            RequestingUserId = userIdStr
        };
        var result = await _mediator.Send(query);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Gets the public profile of an artist by their ID.
    /// </summary>
    /// <param name="id">Artist ID</param>
    /// <returns>Artist profile</returns>
    /// <response code="200">Artist found</response>
    /// <response code="404">Artist not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetArtistaByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return FromServiceResponse(result);
    }

    [HttpGet("by-user/{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var requestingUserId = _currentUser.UserId;
        if (requestingUserId == null || requestingUserId.Value.ToString() != userId)
        {
            return Unauthorized(new ServiceResponse<ArtistaDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "No tienes permisos para ver este perfil",
                        ErrorCode = ServiceResponseMessageType.Auth_Unauthorized,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetArtistaByUserIdQuery
        {
            UserId = userId,
            RequestingUserId = requestingUserId.Value.ToString()
        };
        var result = await _mediator.Send(query);
        return FromServiceResponse(result);
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArtistaCommand command)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<ArtistaDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Token invalido o expirado",
                        ErrorCode = ServiceResponseMessageType.Auth_InvalidToken,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        command.Id = id;
        command.UserId = userId.Value.ToString();
        var result = await _mediator.Send(command);
        return FromServiceResponse(result);
    }
}
