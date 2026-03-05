namespace WePlayRises.Crowdpromotion.Application.Interfaces.Services;

public interface ITrackingRateLimitService
{
    Task<bool> IsRateLimitedAsync(string ipOrigen, string? codigoReferido, CancellationToken ct);
    Task RegisterClickAsync(string ipOrigen, string? codigoReferido, CancellationToken ct);
}
