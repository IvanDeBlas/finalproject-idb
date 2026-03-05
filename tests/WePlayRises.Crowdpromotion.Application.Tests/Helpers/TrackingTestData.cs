using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Helpers;

public static class TrackingTestData
{
    public static readonly string DefaultUserId = Guid.NewGuid().ToString();
    public static readonly PromotorId DefaultPromotorId = new(Guid.NewGuid());
    public static readonly ArtistaId DefaultArtistaId = new(Guid.NewGuid());
    public static readonly PromoProgramaId DefaultProgramaId = PromoProgramaId.CreateNew();
    public static readonly Guid DefaultEventoId = Guid.NewGuid();

    public static RegistrarEventoCommand CreateValidEventoCommand(int tipoEvento = 1)
    {
        return new RegistrarEventoCommand
        {
            CodigoReferido = "REF-TEST-01",
            TipoEventoPromoId = tipoEvento,
            CampaniaCrowdfundingId = Guid.NewGuid(),
            UrlOrigen = "https://example.com/landing",
            UrlReferer = "https://google.com",
            UtmSource = "weplay",
            UtmMedium = "referral",
            UtmCampaign = "PROMO-TEST",
            IpOrigen = "192.168.1.1",
            UserIdAfectado = Guid.NewGuid().ToString()
        };
    }

    public static RegistrarConversionCommand CreateValidConversionCommand()
    {
        return new RegistrarConversionCommand
        {
            CodigoReferido = "REF-TEST-01",
            CampaniaCrowdfundingId = Guid.NewGuid(),
            AportacionCrowdfundingId = Guid.NewGuid(),
            ValorMonetario = 50.00m,
            MonedaId = 1,
            UserIdAfectado = Guid.NewGuid().ToString()
        };
    }

    public static GetProgramaMetricasQuery CreateValidProgramaMetricasQuery()
    {
        return new GetProgramaMetricasQuery
        {
            ProgramaId = DefaultProgramaId.Value,
            UserId = DefaultUserId,
            FechaDesde = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
            FechaHasta = DateOnly.FromDateTime(DateTime.UtcNow)
        };
    }

    public static GetPromotorMetricasQuery CreateValidPromotorMetricasQuery()
    {
        return new GetPromotorMetricasQuery
        {
            UserId = DefaultUserId,
            ProgramaId = DefaultProgramaId.Value,
            FechaDesde = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
            FechaHasta = DateOnly.FromDateTime(DateTime.UtcNow)
        };
    }

    public static RegistrarConversionResult CreateConversionResult(
        Guid? eventoId = null,
        decimal comision = 5.00m,
        bool acreditada = true)
    {
        return new RegistrarConversionResult(
            EventoId: eventoId ?? DefaultEventoId,
            ComisionCalculada: comision,
            WalletTransaccionId: Guid.NewGuid(),
            ComisionAcreditada: acreditada,
            MonedaNombre: "EUR");
    }

    public static ProgramaMetricasResponseDto CreateProgramaMetricasResponse()
    {
        return new ProgramaMetricasResponseDto
        {
            ProgramaId = DefaultProgramaId.Value,
            ProgramaTitulo = "Programa Test",
            FechaDesde = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)).ToString("yyyy-MM-dd"),
            FechaHasta = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
            Kpis = new ProgramaMetricasKpisDto
            {
                TotalClicks = 100,
                TotalPageViews = 200,
                TotalSignups = 10,
                TotalConversiones = 5,
                ValorTotalGenerado = 500m,
                ComisionesTotales = 50m,
                TasaConversion = 2.5m,
                MonedaNombre = "EUR"
            },
            RankingPromotores = new List<RankingPromotorItemDto>(),
            EventosPorDia = new List<EventosPorDiaItemDto>()
        };
    }

    public static PromotorMetricasResponseDto CreatePromotorMetricasResponse()
    {
        return new PromotorMetricasResponseDto
        {
            PromotorId = DefaultPromotorId.Value,
            PromotorNombre = "Test Promotor",
            ProgramaId = DefaultProgramaId.Value,
            ProgramaTitulo = "Programa Test",
            FechaDesde = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)).ToString("yyyy-MM-dd"),
            FechaHasta = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
            Kpis = new PromotorMetricasKpisDto
            {
                MisClicks = 50,
                MisPageViews = 100,
                MisSignups = 5,
                MisConversiones = 3,
                MiValorGenerado = 300m,
                MiComisionAcumulada = 30m,
                MiTasaConversion = 3.0m,
                MonedaNombre = "EUR"
            },
            EventosRecientes = new List<EventoRecienteDto>()
        };
    }

    public static Promotor CreatePromotor()
    {
        return new Promotor
        {
            Id = DefaultPromotorId,
            UserId = DefaultUserId,
            NombrePublico = "Test Promotor",
            TipoPromotorId = 1,
            EsActivo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static PromoPrograma CreatePromoPrograma(ArtistaId? artistaId = null)
    {
        return new PromoPrograma
        {
            Id = DefaultProgramaId,
            ArtistaId = artistaId ?? DefaultArtistaId,
            Titulo = "Programa Test",
            TipoPromoId = 1,
            EsActivo = true,
            MonedaId = 1,
            FechaCreacion = DateTime.UtcNow
        };
    }
}
