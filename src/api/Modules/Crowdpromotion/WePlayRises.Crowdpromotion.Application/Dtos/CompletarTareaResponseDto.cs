namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class CompletarTareaResponseDto
{
    public Guid TareaPromotorId { get; set; }
    public int EstadoTareaId { get; set; }
    public string EstadoTareaNombre { get; set; } = null!;
    public int VecesCompletada { get; set; }
    public DateTime? FechaUltimaCompletada { get; set; }
}
