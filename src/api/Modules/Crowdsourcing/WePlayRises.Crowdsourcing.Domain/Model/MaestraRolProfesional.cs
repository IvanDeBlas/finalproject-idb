namespace WePlayRises.Crowdsourcing.Domain.Model;

public class MaestraRolProfesional
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int CategoriaRolId { get; set; }

    public string? ModalidadCobro { get; set; }

    public bool Activo { get; set; } = true;

    // Navigation properties
    public virtual MaestraCategoriaRol CategoriaRol { get; set; } = null!;
    public virtual ICollection<PlantillaProyectoNecesidad> PlantillasNecesidades { get; set; } = new List<PlantillaProyectoNecesidad>();
}
