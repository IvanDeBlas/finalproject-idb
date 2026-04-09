using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

public class PlantillaProyecto
{
    public PlantillaProyectoId Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Icono { get; set; }

    public int Orden { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual ICollection<PlantillaProyectoNecesidad> Necesidades { get; set; } = new List<PlantillaProyectoNecesidad>();
}
