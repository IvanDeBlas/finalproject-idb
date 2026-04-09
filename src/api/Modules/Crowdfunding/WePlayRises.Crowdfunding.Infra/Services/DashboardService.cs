using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;

namespace WePlayRises.Crowdfunding.Infra.Services;

public class DashboardService : IDashboardService
{
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        ICampaniaRepository campaniaRepository,
        IPedidoRepository pedidoRepository,
        ILogger<DashboardService> logger)
    {
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(decimal TotalRecaudado, int TotalBackers, int CampaniasActivas, int CampaniasCompletadas)>
        GetResumenAsync(ArtistaId artistaId, CancellationToken ct)
    {
        var campanias = await _campaniaRepository.GetByArtistaIdAsync(artistaId, ct);
        var activeCampanias = campanias.Where(c => !c.Borrado).ToList();

        decimal totalRecaudado = activeCampanias.Sum(c => c.ImportePledgedActual);
        int totalBackers = 0;

        foreach (var campania in activeCampanias)
        {
            totalBackers += await _pedidoRepository.CountByCampaniaIdAsync(campania.Id, ct);
        }

        int campaniasActivas = activeCampanias.Count(c => c.EstadoCampaniaId == 2);
        int campaniasCompletadas = activeCampanias.Count(c => c.EstadoCampaniaId == 3);

        return (totalRecaudado, totalBackers, campaniasActivas, campaniasCompletadas);
    }

    public async Task<decimal> GetBackingPromedioAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var stats = await _pedidoRepository.GetStatsByCampaniaIdAsync(campaniaId, ct);
        return stats.Average;
    }

    public async Task<string?> GetRewardMasPopularAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var rewardStats = await _pedidoRepository.GetRewardStatsByCampaniaIdAsync(campaniaId, ct);
        if (rewardStats.Count == 0) return null;

        var topRewardId = rewardStats.OrderByDescending(r => r.Value).First().Key;
        var reward = await _campaniaRepository.GetByIdAsync(campaniaId, ct);

        var rewardEntity = reward?.Rewards?.FirstOrDefault(r => r.Id.Value == topRewardId);
        return rewardEntity?.Nombre;
    }

    public async Task<decimal> GetVelocidadDiariaAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
        if (campania?.FechaInicio == null) return 0;

        var diasTranscurridos = (DateTime.UtcNow - campania.FechaInicio.Value).Days;
        if (diasTranscurridos <= 0) return 0;

        return Math.Round(campania.ImportePledgedActual / diasTranscurridos, 2);
    }

    public async Task<decimal?> CalcularProyeccionFinalAsync(CampaniaCrowdfundingId campaniaId, CancellationToken ct)
    {
        var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
        if (campania?.FechaInicio == null || campania.FechaFin == null) return null;

        var velocidadDiaria = await GetVelocidadDiariaAsync(campaniaId, ct);
        var diasRestantes = Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days);

        return Math.Round(campania.ImportePledgedActual + (velocidadDiaria * diasRestantes), 2);
    }
}
