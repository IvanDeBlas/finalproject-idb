using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;
using PromoTareaEntity = WePlayRises.Crowdpromotion.Domain.Model.PromoTarea;
using static WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands.CreatePromoProgramaCommand;
using static WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands.UpdatePromoProgramaCommand;

namespace WePlayRises.Crowdpromotion.Application.Tests.Helpers;

public static class PromoProgramaTestData
{
    public static readonly string DefaultUserId = Guid.NewGuid().ToString();
    public static readonly ArtistaId DefaultArtistaId = new(Guid.NewGuid());
    public static readonly PromoProgramaId DefaultProgramaId = PromoProgramaId.CreateNew();

    public static CreatePromoProgramaCommand CreateValidCommand(string? userId = null)
    {
        return new CreatePromoProgramaCommand
        {
            Titulo = "Programa Test Promocion",
            Descripcion = "Descripcion del programa de test",
            TipoPromoId = 1,
            MonedaId = 1,
            ImporteComisionPorcentaje = 10m,
            UrlLanding = "https://example.com/landing",
            CodigoTrackingBase = "PROMO-TEST-01",
            FechaInicio = DateOnly.FromDateTime(DateTime.UtcNow),
            FechaFin = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(3)),
            Tareas = new List<CreatePromoTareaItem>
            {
                CreateValidTareaItem()
            },
            UserId = userId ?? DefaultUserId
        };
    }

    public static CreatePromoTareaItem CreateValidTareaItem()
    {
        return new CreatePromoTareaItem
        {
            Titulo = "Tarea Test Uno",
            Descripcion = "Descripcion tarea test",
            TipoEventoPromoId = 1,
            TipoRewardId = 2,
            PuntosRecompensa = 100,
            EsRepetible = true,
            MaxRepeticiones = 5
        };
    }

    public static UpdatePromoProgramaCommand CreateValidUpdateCommand(Guid? id = null, string? userId = null)
    {
        return new UpdatePromoProgramaCommand
        {
            Id = id ?? DefaultProgramaId.Value,
            Titulo = "Programa Actualizado",
            Descripcion = "Descripcion actualizada",
            TipoPromoId = 1,
            MonedaId = 1,
            ImporteComisionFija = 5m,
            UrlLanding = "https://example.com/landing-updated",
            CodigoTrackingBase = "PROMO-UPD-01",
            Tareas = new List<UpdatePromoTareaItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Titulo = "Tarea Actualizada",
                    TipoEventoPromoId = 2,
                    TipoRewardId = 1,
                    ImporteRecompensa = 25m,
                    MonedaId = 1,
                    EsRepetible = false,
                    EsActivo = true
                }
            },
            UserId = userId ?? DefaultUserId
        };
    }

    public static DesactivarPromoProgramaCommand CreateValidDesactivarCommand(Guid? id = null, string? userId = null)
    {
        return new DesactivarPromoProgramaCommand
        {
            Id = id ?? DefaultProgramaId.Value,
            UserId = userId ?? DefaultUserId
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

    public static PromoPrograma CreateValidProgramaWithTareas(
        PromoProgramaId? id = null,
        ArtistaId? artistaId = null)
    {
        var programa = CreateValidPrograma(id, artistaId);
        programa.Tareas = new List<PromoTareaEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProgramaId = programa.Id,
                Titulo = "Tarea 1",
                TipoEventoPromoId = 1,
                TipoRewardId = 2,
                PuntosRecompensa = 100,
                EsActivo = true,
                EsRepetible = true,
                MaxRepeticiones = 5,
                Orden = 1,
                FechaCreacion = DateTime.UtcNow
            }
        };
        return programa;
    }
}
