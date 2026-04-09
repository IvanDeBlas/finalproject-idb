namespace WePlayRises.Crowdfunding.Application.Dtos;

/// <summary>
/// Request DTO for creating a backing (pledge) for a campaign.
/// </summary>
public class CreateBackingRequest
{
    public Guid? RewardId { get; set; }
    public decimal Monto { get; set; }
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }
}
