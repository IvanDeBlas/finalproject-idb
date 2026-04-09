using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Auth.Queries;

public class GetCurrentUserQuery : IRequest<ServiceResponse<UserInfoDto>>
{
    public string UserId { get; set; } = null!;
}

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ServiceResponse<UserInfoDto>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        UserManager<IdentityUser> userManager,
        IMapper mapper,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<UserInfoDto>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        try
        {
            // 1. Find user by ID
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", request.UserId);
                return ValidateExtensions.NotFoundServiceResponse<UserInfoDto>(
                    "Usuario no encontrado",
                    ServiceResponseMessageType.NotFound_User);
            }

            // 2. Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // 3. Map to UserInfoDto
            var userInfo = _mapper.Map<UserInfoDto>(user);
            userInfo.Roles = userRoles.ToList();

            // 4. Return successful ServiceResponse
            return new ServiceResponse<UserInfoDto>
            {
                Data = userInfo,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Usuario recuperado exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user info for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<UserInfoDto>(
                "Error inesperado al obtener informacion del usuario",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
