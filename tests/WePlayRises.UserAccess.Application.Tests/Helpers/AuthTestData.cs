using Microsoft.AspNetCore.Identity;
using WePlayRises.UserAccess.Application.Dtos;

namespace WePlayRises.UserAccess.Application.Tests.Helpers;

public static class AuthTestData
{
    public static IdentityUser CreateValidIdentityUser(
        string? userId = null,
        string email = "test@example.com")
    {
        return new IdentityUser
        {
            Id = userId ?? Guid.NewGuid().ToString(),
            UserName = email,
            Email = email,
            EmailConfirmed = true,
        };
    }

    public static LoginResponseDto CreateLoginResponseDto(
        string? userId = null,
        string email = "test@example.com",
        string token = "jwt-token-123")
    {
        return new LoginResponseDto
        {
            UserId = userId ?? Guid.NewGuid().ToString(),
            Email = email,
            Token = token,
            Roles = new List<string> { "Fan" },
        };
    }

    public static RegisterResponseDto CreateRegisterResponseDto(
        string? userId = null,
        string email = "test@example.com",
        string token = "jwt-token-123")
    {
        return new RegisterResponseDto
        {
            UserId = userId ?? Guid.NewGuid().ToString(),
            Email = email,
            Token = token,
            Roles = new List<string> { "Fan" },
        };
    }

    public static UserInfoDto CreateUserInfoDto(
        string? userId = null,
        string email = "test@example.com")
    {
        return new UserInfoDto
        {
            UserId = userId ?? Guid.NewGuid().ToString(),
            Email = email,
            Roles = new List<string> { "Fan" },
            EmailConfirmed = true,
        };
    }
}
