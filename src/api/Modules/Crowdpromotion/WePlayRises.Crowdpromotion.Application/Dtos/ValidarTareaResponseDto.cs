namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class ValidarTareaResponseDto
{
    public Guid TareaPromotorId { get; set; }
    public int EstadoTareaId { get; set; }
    public string EstadoTareaNombre { get; set; } = null!;
    public decimal? RecompensaAcreditada { get; set; }
    public string? MonedaNombre { get; set; }
    public int? PuntosAcreditados { get; set; }
}
