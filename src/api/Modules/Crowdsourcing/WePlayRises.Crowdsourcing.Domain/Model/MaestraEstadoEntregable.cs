namespace WePlayRises.Crowdsourcing.Domain.Model;

public class MaestraEstadoEntregable
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    // Navigation
    public virtual ICollection<AcuerdoCrowdsourcingEntregable> Entregables { get; set; } = new List<AcuerdoCrowdsourcingEntregable>();
}
