namespace WePlayRises.Crowdsourcing.Domain.Model;

public class MaestraEstadoAcuerdo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    // Navigation
    public virtual ICollection<AcuerdoCrowdsourcing> Acuerdos { get; set; } = new List<AcuerdoCrowdsourcing>();
}
