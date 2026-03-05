using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

public class PlantillaProyectoNecesidad
{
    public PlantillaProyectoNecesidadId Id { get; set; }

    public PlantillaProyectoId PlantillaProyectoId { get; set; }

    public string Fase { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int RolProfesionalId { get; set; }

    public decimal? PrecioMinOrientativo { get; set; }

    public decimal? PrecioMaxOrientativo { get; set; }

    public int MonedaId { get; set; } = 1;

    public string Prioridad { get; set; } = "Media";

    public int Orden { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PlantillaProyecto PlantillaProyecto { get; set; } = null!;
    public virtual MaestraRolProfesional RolProfesional { get; set; } = null!;
}
