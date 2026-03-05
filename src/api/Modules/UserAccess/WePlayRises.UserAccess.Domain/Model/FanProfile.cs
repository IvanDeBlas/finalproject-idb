using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents a fan/supporter profile in the system.
/// </summary>
public class FanProfile
{
    public FanProfileId Id { get; set; }

    /// <summary>
    /// Reference to the Identity User
    /// </summary>
    public string UserId { get; set; } = null!;

    public string? Apodo { get; set; }

    public string? Pais { get; set; }

    public string? Ciudad { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual ICollection<ArtistaFan> ArtistaFans { get; set; } = new List<ArtistaFan>();
}
