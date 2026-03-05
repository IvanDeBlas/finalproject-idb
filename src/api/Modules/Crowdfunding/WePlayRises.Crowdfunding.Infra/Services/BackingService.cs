using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Application.Interfaces.Repositories;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Model;
using WePlayRises.Crowdfunding.Infra.Context;

namespace WePlayRises.Crowdfunding.Infra.Services;

public class BackingService : IBackingService
{
    private readonly CrowdfundingContext _context;
    private readonly ICampaniaRepository _campaniaRepository;
    private readonly ILogger<BackingService> _logger;

    public BackingService(
        CrowdfundingContext context,
        ICampaniaRepository campaniaRepository,
        ILogger<BackingService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _campaniaRepository = campaniaRepository ?? throw new ArgumentNullException(nameof(campaniaRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PedidoCrowdfundingId> CreateBackingAsync(
        CampaniaCrowdfundingId campaniaId,
        CampaniaCrowdfundingRewardId? rewardId,
        decimal monto,
        string? userId,
        string? mensaje,
        bool esAnonimo,
        CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            // 1. Crear PedidoCrowdfunding
            var pedido = new PedidoCrowdfunding
            {
                Id = PedidoCrowdfundingId.CreateNew(),
                CampaniaId = campaniaId,
                UserId = userId,
                EstadoPedidoId = 3, // COMPLETADO (MVP sin pago real)
                MonedaId = 1, // EUR
                ImporteSubtotal = monto,
                ImportePropina = 0,
                ImporteEnvio = 0,
                ImporteImpuestos = 0,
                ImporteTotal = monto,
                PermitirMostrarNombre = !esAnonimo,
                ComentarioBacker = mensaje,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Pedidos.AddAsync(pedido, ct);
            await _context.SaveChangesAsync(ct);

            // 2. Crear PedidoCrowdfundingLinea (solo si hay reward)
            if (rewardId.HasValue)
            {
                var linea = new PedidoCrowdfundingLinea
                {
                    Id = Guid.NewGuid(),
                    PedidoCrowdfundingId = pedido.Id,
                    RewardId = rewardId.Value,
                    Cantidad = 1,
                    PrecioUnitario = monto,
                    ImporteLinea = monto,
                    EsRewardPrincipal = true,
                    FechaCreacion = DateTime.UtcNow
                };

                await _context.PedidoLineas.AddAsync(linea, ct);
            }

            // 3. Actualizar CampaniaCrowdfunding.ImportePledgedActual
            var campania = await _campaniaRepository.GetByIdAsync(campaniaId, ct);
            if (campania == null)
            {
                throw new InvalidOperationException($"Campania {campaniaId.Value} not found during transaction");
            }

            campania.ImportePledgedActual += monto;
            campania.FechaActualizacion = DateTime.UtcNow;
            _context.Campanias.Update(campania);

            // 4. Crear AportacionCrowdfunding (simulated payment for MVP)
            var aportacion = new AportacionCrowdfunding
            {
                Id = AportacionCrowdfundingId.CreateNew(),
                PedidoCrowdfundingId = pedido.Id,
                MonedaId = 1, // EUR
                MetodoPagoId = 99, // Simulated (MVP)
                EstadoAportacionId = 2, // Confirmed
                ImporteTotal = monto,
                ImporteImpuestos = 0,
                ImporteComisionPlataforma = 0,
                ImporteComisionPasarela = 0,
                ImporteNetoArtista = monto,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Aportaciones.AddAsync(aportacion, ct);

            // 5. Final SaveChanges + Commit
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            _logger.LogInformation(
                "Backing created: Pedido {PedidoId}, Campania {CampaniaId}, Monto {Monto}, UserId {UserId}",
                pedido.Id.Value, campaniaId.Value, monto, userId ?? "Anonymous");

            return pedido.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Error creating backing for Campania {CampaniaId}", campaniaId.Value);
            throw;
        }
    }
}
