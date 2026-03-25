namespace WePlayRises.Crowdfunding.Application.Dtos;

public class CampaniaDetailDto
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public decimal ImportePledgedActual { get; set; }
    public decimal PorcentajeProgreso { get; set; }
    public int MonedaId { get; set; }
    public string MonedaSimbolo { get; set; } = null!;
    public int EstadoCampaniaId { get; set; }
    public string EstadoCampaniaNombre { get; set; } = null!;
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int DiasRestantes { get; set; }
    public string ArtistaNombre { get; set; } = null!;
    public string? ArtistaImagenUrl { get; set; }
    public List<RewardPublicDto> Rewards { get; set; } = new();
    public List<BackingPublicDto> BackingsRecientes { get; set; } = new();
    public int TotalBackers { get; set; }
    public DateTime FechaCreacion { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public bool TieneCrowdsourcing { get; set; }
    public bool TieneCrowdpromotion { get; set; }
}
