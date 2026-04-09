namespace WePlayRises.UserAccess.Domain.Constants;

public static class Roles
{
    public const string Artista = "Artista";
    public const string Fan = "Fan";
    public const string Admin = "Admin";

    public static readonly string[] All = { Artista, Fan, Admin };

    public static bool IsValid(string role) => All.Contains(role);
}
