namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromoProgramaCreatedResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string TipoPromoNombre { get; set; } = null!;
    public bool EsActivo { get; set; }
    public int TareasCreadas { get; set; }
    public DateTime FechaCreacion { get; set; }
}
