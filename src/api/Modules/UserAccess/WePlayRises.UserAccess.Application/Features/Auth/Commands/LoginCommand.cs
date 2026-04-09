using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<ServiceResponse<LoginResponseDto>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResponse<LoginResponseDto>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IValidator<LoginCommand> _validator;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IValidator<LoginCommand> validator,
        IJwtTokenGenerator jwtGenerator,
        IMapper mapper,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _jwtGenerator = jwtGenerator ?? throw new ArgumentNullException(nameof(jwtGenerator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        try
        {
            // 1. Validate with FluentValidation
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<LoginResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ValidateExtensions.UnauthorizedServiceResponse<LoginResponseDto>(
                    "Email o contrasena incorrectos",
                    ServiceResponseMessageType.Auth_InvalidCredentials);
            }

            // 3. Verify credentials with SignInManager
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for user {Email}", request.Email);
                return ValidateExtensions.UnauthorizedServiceResponse<LoginResponseDto>(
                    "Email o contrasena incorrectos",
                    ServiceResponseMessageType.Auth_InvalidCredentials);
            }

            // 4. Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // 5. Generate JWT token with roles
            var token = _jwtGenerator.GenerateToken(user.Id, user.Email!, userRoles);

            // 6. Map to LoginResponseDto
            var response = _mapper.Map<LoginResponseDto>(user);
            response.Token = token;
            response.Roles = userRoles.ToList();

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            // 7. Return successful ServiceResponse
            return new ServiceResponse<LoginResponseDto>
            {
                Data = response,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Inicio de sesion exitoso",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user with email {Email}", request.Email);
            return ValidateExtensions.InternalServerErrorServiceResponse<LoginResponseDto>(
                "Error inesperado al iniciar sesion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
