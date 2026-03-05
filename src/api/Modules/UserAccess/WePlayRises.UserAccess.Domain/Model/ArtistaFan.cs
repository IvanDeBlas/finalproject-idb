using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents the relationship between an artist and a fan (follower).
/// </summary>
public class ArtistaFan
{
    public Guid Id { get; set; }

    public ArtistaId ArtistaId { get; set; }

    public FanProfileId FanProfileId { get; set; }

    public bool EsSuperFan { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    // Navigation properties
    public virtual Artista Artista { get; set; } = null!;
    public virtual FanProfile FanProfile { get; set; } = null!;
}
