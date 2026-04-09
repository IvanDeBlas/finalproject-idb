using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoTareaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoTarea;

namespace WePlayRises.Crowdpromotion.Application.Tests.Helpers;

public static class PromoTareaTestData
{
    public static readonly string DefaultUserId = Guid.NewGuid().ToString();
    public static readonly PromotorId DefaultPromotorId = new(Guid.NewGuid());
    public static readonly ArtistaId DefaultArtistaId = new(Guid.NewGuid());
    public static readonly PromoProgramaId DefaultProgramaId = PromoProgramaId.CreateNew();
    public static readonly Guid DefaultTareaId = Guid.NewGuid();
    public static readonly Guid DefaultTareaPromotorId = Guid.NewGuid();
    public static readonly Guid DefaultInscripcionId = Guid.NewGuid();

    public static Promotor CreateValidPromotor(PromotorId? id = null, string? userId = null)
    {
        return new Promotor
        {
            Id = id ?? DefaultPromotorId,
            UserId = userId ?? DefaultUserId,
            TipoPromotorId = 1,
            NombrePublico = "Test Promotor",
            EsActivo = true,
            FechaCreacion = DateTime.UtcNow.AddDays(-30)
        };
    }

    public static PromoPrograma CreateValidPrograma(
        PromoProgramaId? id = null,
        ArtistaId? artistaId = null,
        bool esActivo = true)
    {
        return new PromoPrograma
        {
            Id = id ?? DefaultProgramaId,
            ArtistaId = artistaId ?? DefaultArtistaId,
            Titulo = "Programa Test",
            Descripcion = "Descripcion test",
            TipoPromoId = 1,
            MonedaId = 1,
            ImporteComisionPorcentaje = 10m,
            UrlLanding = "https://example.com/landing",
            CodigoTrackingBase = "PROMO-TEST",
            EsActivo = esActivo,
            FechaCreacion = DateTime.UtcNow.AddDays(-10),
            Tareas = new List<PromoTareaEntity>(),
            Promotores = new List<PromoProgramaPromotor>(),
            Eventos = new List<PromoEvento>()
        };
    }

    public static PromoProgramaPromotor CreateValidInscripcion(
        Guid? id = null,
        bool esAprobado = true,
        bool esBloqueado = false)
    {
        return new PromoProgramaPromotor
        {
            Id = id ?? DefaultInscripcionId,
            ProgramaId = DefaultProgramaId,
            PromotorId = DefaultPromotorId,
            EsAprobado = esAprobado,
            EsBloqueado = esBloqueado,
            EsActivo = true,
            FechaInscripcion = DateTime.UtcNow.AddDays(-5)
        };
    }

    public static PromoTarea CreateValidTarea(
        Guid? id = null,
        bool esActivo = true,
        bool esRepetible = true,
        int? maxRepeticiones = 5,
        DateTime? fechaFin = null)
    {
        return new PromoTarea
        {
            Id = id ?? DefaultTareaId,
            ProgramaId = DefaultProgramaId,
            Titulo = "Tarea Test",
            TipoEventoPromoId = 1,
            TipoRewardId = 2,
            PuntosRecompensa = 100,
            EsActivo = esActivo,
            EsRepetible = esRepetible,
            MaxRepeticiones = maxRepeticiones,
            Orden = 1,
            FechaCreacion = DateTime.UtcNow,
            FechaFin = fechaFin
        };
    }

    public static PromoTareaPromotor CreateValidTareaPromotor(
        Guid? id = null,
        int estadoTareaId = 2)
    {
        return new PromoTareaPromotor
        {
            Id = id ?? DefaultTareaPromotorId,
            TareaId = DefaultTareaId,
            ProgramaPromotorId = DefaultInscripcionId,
            EstadoTareaId = estadoTareaId,
            UrlPruebaCompletado = "https://example.com/proof",
            ComentarioPromotor = "Tarea completada",
            FechaCompletado = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow
        };
    }

    // Commands
    public static CompletarTareaCommand CreateValidCompletarCommand(string? userId = null)
    {
        return new CompletarTareaCommand
        {
            ProgramaId = DefaultProgramaId.Value,
            TareaId = DefaultTareaId,
            UrlPruebaCompletado = "https://example.com/proof",
            ComentarioPromotor = "Prueba completada",
            UserId = userId ?? DefaultUserId
        };
    }

    public static ValidarTareaCommand CreateValidValidarCommand(string? userId = null)
    {
        return new ValidarTareaCommand
        {
            ProgramaId = DefaultProgramaId.Value,
            TareaPromotorId = DefaultTareaPromotorId,
            ComentarioValidacion = "Bien hecho",
            UserId = userId ?? DefaultUserId
        };
    }

    public static RechazarTareaCommand CreateValidRechazarCommand(string? userId = null)
    {
        return new RechazarTareaCommand
        {
            ProgramaId = DefaultProgramaId.Value,
            TareaPromotorId = DefaultTareaPromotorId,
            ComentarioValidacion = "La prueba no es valida",
            UserId = userId ?? DefaultUserId
        };
    }

    // Queries
    public static GetMisTareasQuery CreateValidMisTareasQuery(string? userId = null)
    {
        return new GetMisTareasQuery
        {
            ProgramaId = DefaultProgramaId.Value,
            UserId = userId ?? DefaultUserId
        };
    }

    public static GetTareasPendientesQuery CreateValidTareasPendientesQuery(string? userId = null)
    {
        return new GetTareasPendientesQuery
        {
            ProgramaId = DefaultProgramaId.Value,
            Page = 1,
            PageSize = 10,
            UserId = userId ?? DefaultUserId
        };
    }
}
