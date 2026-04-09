namespace WePlayRises.UserAccess.Application.Dtos;

public class ArtistaListDto
{
    public Guid Id { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
}
