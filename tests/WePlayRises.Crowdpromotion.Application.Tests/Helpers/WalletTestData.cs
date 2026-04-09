using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Helpers;

public static class WalletTestData
{
    public static readonly string DefaultUserId = Guid.NewGuid().ToString();
    public static readonly PromotorId DefaultPromotorId = new(Guid.NewGuid());
    public static readonly Guid DefaultWalletId = Guid.NewGuid();

    public static Promotor CreatePromotor(string? userId = null, PromotorId? id = null)
    {
        return new Promotor
        {
            Id = id ?? DefaultPromotorId,
            UserId = userId ?? DefaultUserId,
            NombrePublico = "Test Promotor",
            TipoPromotorId = 1,
            EsActivo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static PromotorWallet CreateWallet(PromotorId? promotorId = null, Guid? walletId = null)
    {
        return new PromotorWallet
        {
            Id = walletId ?? DefaultWalletId,
            PromotorId = promotorId ?? DefaultPromotorId,
            MonedaId = 1,
            SaldoDisponible = 100.00m,
            SaldoPendiente = 20.00m,
            TotalGanado = 200.00m,
            TotalRetirado = 80.00m,
            FechaCreacion = DateTime.UtcNow,
            RowVersion = new byte[] { 0, 0, 0, 1 }
        };
    }

    public static PromotorWalletTransaccion CreateTransaccion(
        Guid? walletId = null,
        bool esCredito = true,
        decimal importe = 10.00m,
        int estadoTransaccionId = 2)
    {
        return new PromotorWalletTransaccion
        {
            Id = Guid.NewGuid(),
            WalletId = walletId ?? DefaultWalletId,
            EsCredito = esCredito,
            Importe = importe,
            Concepto = esCredito ? "Comision" : "Retiro",
            Descripcion = "Test transaccion",
            EstadoTransaccionId = estadoTransaccionId,
            TipoRewardId = esCredito ? 1 : null,
            PromoEventoId = esCredito ? Guid.NewGuid() : null,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static List<PromotorWalletTransaccion> CreateTransacciones(int count = 3, Guid? walletId = null)
    {
        var list = new List<PromotorWalletTransaccion>();
        for (int i = 0; i < count; i++)
        {
            list.Add(CreateTransaccion(walletId: walletId, importe: 10.00m * (i + 1)));
        }
        return list;
    }

    public static GetPromotorWalletQuery CreateGetWalletQuery(string? userId = null)
    {
        return new GetPromotorWalletQuery
        {
            UserId = userId ?? DefaultUserId
        };
    }

    public static GetWalletTransaccionesQuery CreateGetTransaccionesQuery(string? userId = null)
    {
        return new GetWalletTransaccionesQuery
        {
            UserId = userId ?? DefaultUserId,
            Page = 1,
            PageSize = 10
        };
    }

    public static SolicitarCobroCommand CreateSolicitarCobroCommand(
        string? userId = null,
        decimal importe = 25.00m,
        string? descripcion = null)
    {
        return new SolicitarCobroCommand
        {
            UserId = userId ?? DefaultUserId,
            Importe = importe,
            Descripcion = descripcion
        };
    }

    public static SolicitarCobroResult CreateCobroResult(
        decimal importe = 25.00m,
        decimal saldoRestante = 75.00m)
    {
        return new SolicitarCobroResult(
            TransaccionId: Guid.NewGuid(),
            Importe: importe,
            MonedaNombre: "EUR",
            SaldoRestante: saldoRestante,
            FechaCreacion: DateTime.UtcNow);
    }
}
