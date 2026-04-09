using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents an artistic project (album, single, tour, etc.).
/// </summary>
public class ProyectoArtistico
{
    public ProyectoArtisticoId Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    /// <summary>
    /// Reference to MaestraTipoProyecto
    /// </summary>
    public int TipoProyectoId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? DescripcionCorta { get; set; }

    public string? DescripcionLarga { get; set; }

    public string? UrlPortada { get; set; }

    /// <summary>
    /// Reference to MaestraEstadoProyecto
    /// </summary>
    public int EstadoProyectoId { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFinPrevista { get; set; }

    public DateTime? FechaFinReal { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual Artista Artista { get; set; } = null!;
}
