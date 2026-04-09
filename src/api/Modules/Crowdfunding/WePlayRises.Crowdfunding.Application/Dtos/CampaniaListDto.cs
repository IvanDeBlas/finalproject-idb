namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO simplificado para listados de campanias
/// </summary>
public class CampaniaListDto
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal ImporteObjetivo { get; set; }
    public decimal ImportePledgedActual { get; set; }
    public int EstadoCampaniaId { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public bool TieneCrowdsourcing { get; set; }
    public bool TieneCrowdpromotion { get; set; }
}
