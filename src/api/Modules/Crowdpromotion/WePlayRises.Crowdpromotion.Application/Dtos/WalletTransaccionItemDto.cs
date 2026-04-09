namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class WalletTransaccionItemDto
{
    public Guid Id { get; set; }
    public bool EsCredito { get; set; }
    public decimal Importe { get; set; }
    public string? Descripcion { get; set; }
    public string? Concepto { get; set; }
    public int EstadoTransaccionId { get; set; }
    public string EstadoTransaccionNombre { get; set; } = null!;
    public int? TipoRewardId { get; set; }
    public string? TipoRewardNombre { get; set; }
    public Guid? PromoEventoId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaProcesado { get; set; }
}
