namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorWalletDto
{
    public Guid WalletId { get; set; }
    public int MonedaId { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public decimal SaldoDisponible { get; set; }
    public decimal SaldoPendiente { get; set; }
    public decimal TotalGanado { get; set; }
    public decimal TotalRetirado { get; set; }
    public decimal MinimoRetiro { get; set; }
}
