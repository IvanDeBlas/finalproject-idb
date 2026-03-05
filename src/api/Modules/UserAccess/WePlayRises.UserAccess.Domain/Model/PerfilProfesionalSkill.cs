using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.UserAccess.Domain.Model;

/// <summary>
/// Represents a skill associated with a professional profile.
/// </summary>
public class PerfilProfesionalSkill
{
    public Guid Id { get; set; }

    public PerfilProfesionalId PerfilProfesionalId { get; set; }

    /// <summary>
    /// Reference to MaestraTipoSkill
    /// </summary>
    public int TipoSkillId { get; set; }

    /// <summary>
    /// Skill level (1-5)
    /// </summary>
    public byte Nivel { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PerfilProfesional PerfilProfesional { get; set; } = null!;
}
