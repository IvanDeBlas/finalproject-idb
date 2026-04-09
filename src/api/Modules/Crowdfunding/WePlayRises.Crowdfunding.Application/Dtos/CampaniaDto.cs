namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO completo para CampaniaCrowdfunding
/// </summary>
public class CampaniaDto
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public int MonedaId { get; set; }
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public decimal ImportePledgedActual { get; set; }
    public int TipoFinanciacionId { get; set; }
    public int EstadoCampaniaId { get; set; }
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public decimal? PorcentajeComisionPlataforma { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
