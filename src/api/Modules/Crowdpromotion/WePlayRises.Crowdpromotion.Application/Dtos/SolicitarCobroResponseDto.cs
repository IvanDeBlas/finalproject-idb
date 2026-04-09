namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class SolicitarCobroResponseDto
{
    public Guid TransaccionId { get; set; }
    public decimal Importe { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public string EstadoTransaccionNombre { get; set; } = null!;
    public decimal SaldoRestante { get; set; }
    public DateTime FechaCreacion { get; set; }
}
