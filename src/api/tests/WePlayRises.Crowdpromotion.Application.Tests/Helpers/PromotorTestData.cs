using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Helpers;

public static class PromotorTestData
{
    public static readonly string DefaultUserId = Guid.NewGuid().ToString();
    public static readonly PromotorId DefaultPromotorId = PromotorId.CreateNew();

    public static Promotor CreateValidPromotor(
        PromotorId? id = null,
        string? userId = null,
        string nombrePublico = "Test Promotor",
        int tipoPromotorId = 2,
        bool esActivo = true)
    {
        return new Promotor
        {
            Id = id ?? DefaultPromotorId,
            UserId = userId ?? DefaultUserId,
            NombrePublico = nombrePublico,
            TipoPromotorId = tipoPromotorId,
            EmailContacto = "test@example.com",
            UrlSitioWeb = "https://example.com",
            UrlInstagram = "https://instagram.com/test",
            EsActivo = esActivo,
            FechaCreacion = DateTime.UtcNow.AddDays(-10),
            FechaActualizacion = null
        };
    }

    public static CreatePromotorCommand CreateValidCommand(string? userId = null)
    {
        return new CreatePromotorCommand
        {
            NombrePublico = "DJ Marketing Pro",
            TipoPromotorId = 2,
            EmailContacto = "dj@example.com",
            UrlSitioWeb = "https://djmarketing.com",
            UrlInstagram = "https://instagram.com/djpro",
            UserId = userId ?? DefaultUserId
        };
    }

    public static UpdatePromotorCommand CreateValidUpdateCommand(string? userId = null)
    {
        return new UpdatePromotorCommand
        {
            NombrePublico = "DJ Marketing Pro Updated",
            EmailContacto = "new@example.com",
            UrlSitioWeb = "https://newsite.com",
            UserId = userId ?? DefaultUserId
        };
    }

    public static PromotorWallet CreateWalletEur(PromotorId promotorId)
    {
        return new PromotorWallet
        {
            Id = Guid.NewGuid(),
            PromotorId = promotorId,
            MonedaId = 1,
            SaldoDisponible = 100.50m,
            SaldoPendiente = 25.00m,
            TotalGanado = 500.00m,
            TotalRetirado = 374.50m,
            FechaCreacion = DateTime.UtcNow.AddDays(-5)
        };
    }
}
