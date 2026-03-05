using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents a member of an artist group/band.
/// </summary>
public class ArtistaMiembro
{
    public Guid Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    /// <summary>
    /// Reference to the Identity User
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Reference to MaestraRolMiembroArtista
    /// </summary>
    public int RolMiembroId { get; set; }

    public bool EsAdmin { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaBaja { get; set; }

    // Navigation properties
    public virtual Artista Artista { get; set; } = null!;
}
