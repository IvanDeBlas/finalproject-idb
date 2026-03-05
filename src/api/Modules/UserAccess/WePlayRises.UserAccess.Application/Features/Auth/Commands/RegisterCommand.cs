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

public class RegisterCommand : IRequest<ServiceResponse<RegisterResponseDto>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string? Role { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ServiceResponse<RegisterResponseDto>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IValidator<RegisterCommand> _validator;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UserManager<IdentityUser> userManager,
        IValidator<RegisterCommand> validator,
        IJwtTokenGenerator jwtGenerator,
        IMapper mapper,
        ILogger<RegisterCommandHandler> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _jwtGenerator = jwtGenerator ?? throw new ArgumentNullException(nameof(jwtGenerator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RegisterResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<RegisterResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return ValidateExtensions.ConflictServiceResponse<RegisterResponseDto>(
                    "Este email ya esta registrado",
                    ServiceResponseMessageType.Validation_DuplicateEmail);
            }

            // 3. Create user with Identity
            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new ServiceResponse<RegisterResponseDto>
                {
                    Messages = result.Errors.Select(e => new ServiceResponseMessage
                    {
                        Message = e.Description,
                        ErrorCode = "AUTH_IDENTITY_ERROR",
                        HttpStatusCode = System.Net.HttpStatusCode.BadRequest
                    }).ToList()
                };
            }

            // 3.1 Assign default role (Fan if not specified)
            var roleToAssign = string.IsNullOrEmpty(request.Role) ? Roles.Fan : request.Role;
            await _userManager.AddToRoleAsync(user, roleToAssign);

            _logger.LogInformation("User {UserId} registered with role {Role}", user.Id, roleToAssign);

            // 4. Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // 5. Generate JWT token with roles
            var token = _jwtGenerator.GenerateToken(user.Id, user.Email!, userRoles);

            // 6. Map and return response
            var response = _mapper.Map<RegisterResponseDto>(user);
            response.Token = token;
            response.Roles = userRoles.ToList();

            _logger.LogInformation("User {UserId} registered successfully with email {Email}",
                user.Id, user.Email);

            return new ServiceResponse<RegisterResponseDto>
            {
                Data = response,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Usuario registrado exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user with email {Email}", request.Email);
            return ValidateExtensions.InternalServerErrorServiceResponse<RegisterResponseDto>(
                "Error inesperado al crear cuenta",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
