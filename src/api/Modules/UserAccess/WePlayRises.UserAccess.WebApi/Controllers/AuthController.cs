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
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Application.Features.Auth.Queries;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AuthController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<AuthController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="command">Registration data (email, password, confirmPassword)</param>
    /// <returns>User information and JWT token</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Validation errors</response>
    /// <response code="409">Email already exists</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Authenticate user with email and password.
    /// </summary>
    /// <param name="command">Login credentials (email, password)</param>
    /// <returns>User information and JWT token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Validation errors</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Get current authenticated user information.
    /// </summary>
    /// <returns>User information and roles</returns>
    /// <response code="200">User information retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = _currentUser.UserId;
        if (userId == null)
        {
            return Unauthorized(new ServiceResponse<UserInfoDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Usuario no autenticado",
                        ErrorCode = ServiceResponseMessageType.Auth_UserNotAuthenticated,
                        HttpStatusCode = HttpStatusCode.Unauthorized
                    }
                }
            });
        }

        var query = new GetCurrentUserQuery { UserId = userId.Value.ToString() };
        var result = await _mediator.Send(query);
        return FromServiceResponse(result);
    }
}
