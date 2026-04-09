namespace WePlayRises.UserAccess.Application.Dtos;

public class ArtistaDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
