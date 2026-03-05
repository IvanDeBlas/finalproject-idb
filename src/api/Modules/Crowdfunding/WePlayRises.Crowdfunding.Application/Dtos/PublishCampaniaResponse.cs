namespace WePlayRises.Crowdfunding.Application.Dtos;

public class PublishCampaniaResponse
{
    public Guid Id { get; set; }
    public int EstadoCampaniaId { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public string Message { get; set; } = null!;
}
