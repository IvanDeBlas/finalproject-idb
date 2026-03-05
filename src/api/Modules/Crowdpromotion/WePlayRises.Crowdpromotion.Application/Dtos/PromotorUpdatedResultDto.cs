namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorUpdatedResultDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public DateTime FechaActualizacion { get; set; }
}
