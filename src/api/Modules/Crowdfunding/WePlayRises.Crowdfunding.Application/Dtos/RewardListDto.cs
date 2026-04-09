namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// DTO simplificado para listados de rewards
/// </summary>
public class RewardListDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public bool EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; }
    public bool IncluyeEnvioFisico { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
}
