namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorCreatedResultDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public string TipoPromotorNombre { get; set; } = null!;
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
