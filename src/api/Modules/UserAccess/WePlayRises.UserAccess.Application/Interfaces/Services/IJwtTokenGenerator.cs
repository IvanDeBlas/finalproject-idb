namespace WePlayRises.UserAccess.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, IList<string> roles);
}
