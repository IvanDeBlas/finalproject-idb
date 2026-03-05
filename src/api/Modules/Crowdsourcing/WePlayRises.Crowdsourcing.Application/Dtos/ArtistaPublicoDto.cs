namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ArtistaPublicoDto
{
    public Guid Id { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public string? ImagenUrl { get; set; }
}
