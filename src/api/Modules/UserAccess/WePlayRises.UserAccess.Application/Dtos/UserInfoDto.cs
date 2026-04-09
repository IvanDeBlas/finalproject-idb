namespace WePlayRises.UserAccess.Application.Dtos;

public class UserInfoDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public bool EmailConfirmed { get; set; }
}
