using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Tests.Helpers;

public static class InscripcionTestData
{
    public static readonly string DefaultUserId = Guid.NewGuid().ToString();
    public static readonly PromotorId DefaultPromotorId = PromotorId.CreateNew();
    public static readonly PromoProgramaId DefaultProgramaId = PromoProgramaId.CreateNew();
    public static readonly ArtistaId DefaultArtistaId = new(Guid.NewGuid());

    public static Promotor CreateValidPromotor(
        PromotorId? id = null,
        string? userId = null,
        bool esActivo = true)
    {
        return new Promotor
        {
            Id = id ?? DefaultPromotorId,
            UserId = userId ?? DefaultUserId,
            NombrePublico = "Test Promotor",
            TipoPromotorId = 2,
            EmailContacto = "test@example.com",
            EsActivo = esActivo,
            FechaCreacion = DateTime.UtcNow.AddDays(-10)
        };
    }

    public static PromoPrograma CreateValidPrograma(
        PromoProgramaId? id = null,
        ArtistaId? artistaId = null,
        bool esActivo = true,
        string? codigoTrackingBase = "PROMO1",
        string? urlLanding = "https://example.com/landing")
    {
        return new PromoPrograma
        {
            Id = id ?? DefaultProgramaId,
            ArtistaId = artistaId ?? DefaultArtistaId,
            Titulo = "Test Programa",
            TipoPromoId = 1,
            EsActivo = esActivo,
            CodigoTrackingBase = codigoTrackingBase,
            UrlLanding = urlLanding,
            FechaCreacion = DateTime.UtcNow.AddDays(-5)
        };
    }

    public static PromoProgramaPromotor CreatePendienteInscripcion(
        Guid? id = null,
        PromoProgramaId? programaId = null,
        PromotorId? promotorId = null)
    {
        return new PromoProgramaPromotor
        {
            Id = id ?? Guid.NewGuid(),
            ProgramaId = programaId ?? DefaultProgramaId,
            PromotorId = promotorId ?? DefaultPromotorId,
            EsAprobado = false,
            EsBloqueado = false,
            EsActivo = true,
            FechaInscripcion = DateTime.UtcNow.AddDays(-1),
            Promotor = CreateValidPromotor()
        };
    }

    public static PromoProgramaPromotor CreateAprobadaInscripcion(
        Guid? id = null,
        PromoProgramaId? programaId = null,
        PromotorId? promotorId = null)
    {
        return new PromoProgramaPromotor
        {
            Id = id ?? Guid.NewGuid(),
            ProgramaId = programaId ?? DefaultProgramaId,
            PromotorId = promotorId ?? DefaultPromotorId,
            EsAprobado = true,
            EsBloqueado = false,
            EsActivo = true,
            CodigoReferido = "PROMO1-abc12",
            UrlReferido = "https://example.com/landing?ref=PROMO1-abc12",
            FechaInscripcion = DateTime.UtcNow.AddDays(-3),
            Promotor = CreateValidPromotor()
        };
    }

    public static PromoProgramaPromotor CreateBloqueadaInscripcion(
        Guid? id = null,
        PromoProgramaId? programaId = null,
        PromotorId? promotorId = null)
    {
        return new PromoProgramaPromotor
        {
            Id = id ?? Guid.NewGuid(),
            ProgramaId = programaId ?? DefaultProgramaId,
            PromotorId = promotorId ?? DefaultPromotorId,
            EsAprobado = false,
            EsBloqueado = true,
            EsActivo = true,
            FechaInscripcion = DateTime.UtcNow.AddDays(-5),
            Promotor = CreateValidPromotor()
        };
    }

    public static SolicitarInscripcionCommand CreateSolicitarCommand(
        Guid? programaId = null, string? userId = null)
    {
        return new SolicitarInscripcionCommand
        {
            ProgramaId = programaId ?? DefaultProgramaId.Value,
            UserId = userId ?? DefaultUserId
        };
    }

    public static AprobarInscripcionCommand CreateAprobarCommand(
        Guid? programaId = null, Guid? inscripcionId = null, string? userId = null)
    {
        return new AprobarInscripcionCommand
        {
            ProgramaId = programaId ?? DefaultProgramaId.Value,
            InscripcionId = inscripcionId ?? Guid.NewGuid(),
            UserId = userId ?? DefaultUserId
        };
    }

    public static RechazarInscripcionCommand CreateRechazarCommand(
        Guid? programaId = null, Guid? inscripcionId = null, string? userId = null)
    {
        return new RechazarInscripcionCommand
        {
            ProgramaId = programaId ?? DefaultProgramaId.Value,
            InscripcionId = inscripcionId ?? Guid.NewGuid(),
            UserId = userId ?? DefaultUserId
        };
    }

    public static BloquearInscripcionCommand CreateBloquearCommand(
        Guid? programaId = null, Guid? inscripcionId = null, string? userId = null)
    {
        return new BloquearInscripcionCommand
        {
            ProgramaId = programaId ?? DefaultProgramaId.Value,
            InscripcionId = inscripcionId ?? Guid.NewGuid(),
            UserId = userId ?? DefaultUserId
        };
    }

    public static DarDeBajaInscripcionCommand CreateDarDeBajaCommand(
        Guid? programaId = null, Guid? inscripcionId = null, string? userId = null)
    {
        return new DarDeBajaInscripcionCommand
        {
            ProgramaId = programaId ?? DefaultProgramaId.Value,
            InscripcionId = inscripcionId ?? Guid.NewGuid(),
            UserId = userId ?? DefaultUserId
        };
    }
}
