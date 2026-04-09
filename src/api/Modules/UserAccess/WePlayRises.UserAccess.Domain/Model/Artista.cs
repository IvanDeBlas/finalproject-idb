using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents an artist profile in the system.
/// </summary>
public class Artista
{
    public ArtistaId Id { get; set; }

    /// <summary>
    /// Reference to the Identity User (owner)
    /// </summary>
    public string UserIdPropietario { get; set; } = null!;

    public string NombreArtistico { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Pais { get; set; }

    public string? Ciudad { get; set; }

    public string? ImagenPerfilUrl { get; set; }

    public string? UrlSitioWeb { get; set; }

    public string? UrlInstagram { get; set; }

    public string? UrlYouTube { get; set; }

    public string? UrlSpotify { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    // Navigation properties
    public virtual ICollection<ArtistaMiembro> ArtistaMiembros { get; set; } = new List<ArtistaMiembro>();
    public virtual ICollection<ArtistaFan> ArtistaFans { get; set; } = new List<ArtistaFan>();
    public virtual ICollection<ProyectoArtistico> ProyectosArtisticos { get; set; } = new List<ProyectoArtistico>();
}
