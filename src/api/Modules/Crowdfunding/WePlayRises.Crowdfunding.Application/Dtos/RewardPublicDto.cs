namespace WePlayRises.Crowdfunding.Application.Dtos;

public class RewardPublicDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public int? CantidadMaxima { get; set; }
    public int CantidadVendida { get; set; }
    public bool Disponible { get; set; }
    public bool IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
}
