namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class RegistrarConversionResponseDto
{
    public Guid EventoId { get; set; }
    public decimal ComisionCalculada { get; set; }
    public string? MonedaNombre { get; set; }
    public Guid? WalletTransaccionId { get; set; }
    public bool ComisionAcreditada { get; set; }
}
