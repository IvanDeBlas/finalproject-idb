namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class RechazarTareaResponseDto
{
    public Guid TareaPromotorId { get; set; }
    public int EstadoTareaId { get; set; }
    public string EstadoTareaNombre { get; set; } = null!;
}
