namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Request DTO for creating a campaign. Does NOT include ArtistaId (extracted from JWT token).
/// </summary>
public class CreateCampaniaRequest
{
    public Guid? ProyectoArtisticoId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public int MonedaId { get; set; } = 1;
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int TipoFinanciacionId { get; set; }
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
