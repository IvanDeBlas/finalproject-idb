namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO completo para CampaniaCrowdfundingReward
/// </summary>
public class RewardDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public int TipoRewardId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public int MonedaId { get; set; }
    public bool EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; }
    public int? CantidadPorBacker { get; set; }
    public bool IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
