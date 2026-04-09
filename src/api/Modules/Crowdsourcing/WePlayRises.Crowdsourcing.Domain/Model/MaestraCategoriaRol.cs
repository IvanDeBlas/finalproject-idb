namespace WePlayRises.Crowdsourcing.Domain.Model;

public class MaestraCategoriaRol
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Icono { get; set; }

    public int Orden { get; set; }

    // Navigation properties
    public virtual ICollection<MaestraRolProfesional> Roles { get; set; } = new List<MaestraRolProfesional>();
}
