using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.Core.Domain.Model.Maestras;

namespace WePlayRises.Core.Infra.Context;

public class CoreContext : CoreDbContext
{
    public CoreContext(DbContextOptions<CoreContext> options) : base(options)
    {
    }

    #region Maestras DbSets

    public DbSet<MaestraMoneda> MaestraMoneda => Set<MaestraMoneda>();
    public DbSet<MaestraMetodoPago> MaestraMetodoPago => Set<MaestraMetodoPago>();
    public DbSet<MaestraModalidadTrabajo> MaestraModalidadTrabajo => Set<MaestraModalidadTrabajo>();

    // Crowdfunding
    public DbSet<MaestraEstadoCampaniaCrowd> MaestraEstadoCampaniaCrowd => Set<MaestraEstadoCampaniaCrowd>();
    public DbSet<MaestraEstadoPedidoCrowd> MaestraEstadoPedidoCrowd => Set<MaestraEstadoPedidoCrowd>();
    public DbSet<MaestraEstadoAportacionCrowd> MaestraEstadoAportacionCrowd => Set<MaestraEstadoAportacionCrowd>();
    public DbSet<MaestraEstadoPayoutCrowd> MaestraEstadoPayoutCrowd => Set<MaestraEstadoPayoutCrowd>();
    public DbSet<MaestraTipoFinanciacion> MaestraTipoFinanciacion => Set<MaestraTipoFinanciacion>();
    public DbSet<MaestraTipoReward> MaestraTipoReward => Set<MaestraTipoReward>();

    // UserAccess
    public DbSet<MaestraEstadoProyecto> MaestraEstadoProyecto => Set<MaestraEstadoProyecto>();
    public DbSet<MaestraTipoProyecto> MaestraTipoProyecto => Set<MaestraTipoProyecto>();
    public DbSet<MaestraTipoSkill> MaestraTipoSkill => Set<MaestraTipoSkill>();
    public DbSet<MaestraRolMiembroArtista> MaestraRolMiembroArtista => Set<MaestraRolMiembroArtista>();

    // Crowdsourcing
    public DbSet<MaestraEstadoNecesidad> MaestraEstadoNecesidad => Set<MaestraEstadoNecesidad>();
    public DbSet<MaestraEstadoPropuesta> MaestraEstadoPropuesta => Set<MaestraEstadoPropuesta>();
    public DbSet<MaestraEstadoAcuerdo> MaestraEstadoAcuerdo => Set<MaestraEstadoAcuerdo>();
    public DbSet<MaestraEstadoEntregable> MaestraEstadoEntregable => Set<MaestraEstadoEntregable>();
    public DbSet<MaestraTipoNecesidad> MaestraTipoNecesidad => Set<MaestraTipoNecesidad>();
    public DbSet<MaestraTipoValoracion> MaestraTipoValoracion => Set<MaestraTipoValoracion>();

    // Crowdpromotion
    public DbSet<MaestraTipoPromo> MaestraTipoPromo => Set<MaestraTipoPromo>();
    public DbSet<MaestraTipoPromotor> MaestraTipoPromotor => Set<MaestraTipoPromotor>();
    public DbSet<MaestraTipoEventoPromo> MaestraTipoEventoPromo => Set<MaestraTipoEventoPromo>();
    public DbSet<MaestraEstadoTareaPromo> MaestraEstadoTareaPromo => Set<MaestraEstadoTareaPromo>();
    public DbSet<MaestraEstadoWalletTransaccion> MaestraEstadoWalletTransaccion => Set<MaestraEstadoWalletTransaccion>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreContext).Assembly);

        // Configure all Maestra tables with common settings
        ConfigureMaestraTables(modelBuilder);

        // Seed MVP master data
        SeedMaestraData(modelBuilder);
    }

    private void ConfigureMaestraTables(ModelBuilder modelBuilder)
    {
        // MaestraMoneda - with Symbol
        modelBuilder.Entity<MaestraMoneda>(entity =>
        {
            entity.ToTable("Maestra_Moneda");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Codigo).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Simbolo).HasMaxLength(5);
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // MaestraMetodoPago
        modelBuilder.Entity<MaestraMetodoPago>(entity =>
        {
            entity.ToTable("Maestra_MetodoPago");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // MaestraModalidadTrabajo
        modelBuilder.Entity<MaestraModalidadTrabajo>(entity =>
        {
            entity.ToTable("Maestra_ModalidadTrabajo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // Estados Crowdfunding
        ConfigureBaseMaestra<MaestraEstadoCampaniaCrowd>(modelBuilder, "Maestra_EstadoCampaniaCrowd");
        ConfigureBaseMaestra<MaestraEstadoPedidoCrowd>(modelBuilder, "Maestra_EstadoPedidoCrowd");
        ConfigureBaseMaestra<MaestraEstadoAportacionCrowd>(modelBuilder, "Maestra_EstadoAportacionCrowd");
        ConfigureBaseMaestra<MaestraEstadoPayoutCrowd>(modelBuilder, "Maestra_EstadoPayoutCrowd");
        ConfigureBaseMaestra<MaestraTipoFinanciacion>(modelBuilder, "Maestra_TipoFinanciacion");
        ConfigureBaseMaestra<MaestraTipoReward>(modelBuilder, "Maestra_TipoReward");

        // Estados UserAccess
        ConfigureBaseMaestra<MaestraEstadoProyecto>(modelBuilder, "Maestra_EstadoProyecto");
        ConfigureBaseMaestra<MaestraTipoProyecto>(modelBuilder, "Maestra_TipoProyecto");
        ConfigureBaseMaestra<MaestraTipoSkill>(modelBuilder, "Maestra_TipoSkill");
        ConfigureBaseMaestra<MaestraRolMiembroArtista>(modelBuilder, "Maestra_RolMiembroArtista");

        // Estados Crowdsourcing
        ConfigureBaseMaestra<MaestraEstadoNecesidad>(modelBuilder, "Maestra_EstadoNecesidad");
        ConfigureBaseMaestra<MaestraEstadoPropuesta>(modelBuilder, "Maestra_EstadoPropuesta");
        ConfigureBaseMaestra<MaestraEstadoAcuerdo>(modelBuilder, "Maestra_EstadoAcuerdo");
        ConfigureBaseMaestra<MaestraEstadoEntregable>(modelBuilder, "Maestra_EstadoEntregable");
        ConfigureBaseMaestra<MaestraTipoNecesidad>(modelBuilder, "Maestra_TipoNecesidad");
        ConfigureBaseMaestra<MaestraTipoValoracion>(modelBuilder, "Maestra_TipoValoracion");

        // Estados Crowdpromotion
        ConfigureBaseMaestra<MaestraTipoPromo>(modelBuilder, "Maestra_TipoPromo");
        ConfigureBaseMaestra<MaestraTipoPromotor>(modelBuilder, "Maestra_TipoPromotor");
        ConfigureBaseMaestra<MaestraTipoEventoPromo>(modelBuilder, "Maestra_TipoEventoPromo");
        ConfigureBaseMaestra<MaestraEstadoTareaPromo>(modelBuilder, "Maestra_EstadoTareaPromo");
        ConfigureBaseMaestra<MaestraEstadoWalletTransaccion>(modelBuilder, "Maestra_EstadoWalletTransaccion");
    }

    private static void ConfigureBaseMaestra<T>(ModelBuilder modelBuilder, string tableName)
        where T : Domain.Model.BaseMaestra
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.ToTable(tableName);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });
    }

    private static void SeedMaestraData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // MaestraEstadoCampaniaCrowd
        modelBuilder.Entity<MaestraEstadoCampaniaCrowd>().HasData(
            new MaestraEstadoCampaniaCrowd { Id = 1, Codigo = "BORRADOR", Nombre = "Borrador", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoCampaniaCrowd { Id = 2, Codigo = "EN_REVISION", Nombre = "En Revision", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoCampaniaCrowd { Id = 3, Codigo = "ACTIVA", Nombre = "Activa", Orden = 3, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoCampaniaCrowd { Id = 4, Codigo = "FINALIZADA_EXITO", Nombre = "Finalizada con Exito", Orden = 4, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoCampaniaCrowd { Id = 5, Codigo = "FINALIZADA_FRACASO", Nombre = "Finalizada sin Exito", Orden = 5, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoCampaniaCrowd { Id = 6, Codigo = "CANCELADA", Nombre = "Cancelada", Orden = 6, EsActivo = true, FechaCreacion = seedDate }
        );

        // MaestraTipoFinanciacion
        modelBuilder.Entity<MaestraTipoFinanciacion>().HasData(
            new MaestraTipoFinanciacion { Id = 1, Codigo = "TODO_O_NADA", Nombre = "Todo o Nada", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraTipoFinanciacion { Id = 2, Codigo = "FLEXIBLE", Nombre = "Flexible", Orden = 2, EsActivo = true, FechaCreacion = seedDate }
        );

        // MaestraMoneda
        modelBuilder.Entity<MaestraMoneda>().HasData(
            new MaestraMoneda { Id = 1, Codigo = "EUR", Nombre = "Euro", Simbolo = "EUR", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraMoneda { Id = 2, Codigo = "USD", Nombre = "US Dollar", Simbolo = "$", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
            new MaestraMoneda { Id = 3, Codigo = "GBP", Nombre = "British Pound", Simbolo = "GBP", Orden = 3, EsActivo = true, FechaCreacion = seedDate }
        );

        // MaestraTipoReward
        modelBuilder.Entity<MaestraTipoReward>().HasData(
            new MaestraTipoReward { Id = 1, Codigo = "FISICO", Nombre = "Fisico", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraTipoReward { Id = 2, Codigo = "DIGITAL", Nombre = "Digital", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
            new MaestraTipoReward { Id = 3, Codigo = "EXPERIENCIA", Nombre = "Experiencia", Orden = 3, EsActivo = true, FechaCreacion = seedDate },
            new MaestraTipoReward { Id = 4, Codigo = "NO_REWARD", Nombre = "Sin Recompensa", Orden = 4, EsActivo = true, FechaCreacion = seedDate }
        );

        // MaestraMetodoPago
        modelBuilder.Entity<MaestraMetodoPago>().HasData(
            new MaestraMetodoPago { Id = 1, Codigo = "TARJETA", Nombre = "Tarjeta de Credito", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraMetodoPago { Id = 2, Codigo = "PAYPAL", Nombre = "PayPal", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
            new MaestraMetodoPago { Id = 3, Codigo = "STRIPE", Nombre = "Stripe", Orden = 3, EsActivo = true, FechaCreacion = seedDate }
        );

        // MaestraEstadoPedidoCrowd
        modelBuilder.Entity<MaestraEstadoPedidoCrowd>().HasData(
            new MaestraEstadoPedidoCrowd { Id = 1, Codigo = "PENDIENTE", Nombre = "Pendiente", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoPedidoCrowd { Id = 2, Codigo = "CONFIRMADO", Nombre = "Confirmado", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoPedidoCrowd { Id = 3, Codigo = "CANCELADO", Nombre = "Cancelado", Orden = 3, EsActivo = true, FechaCreacion = seedDate }
        );

        // MaestraEstadoAportacionCrowd
        modelBuilder.Entity<MaestraEstadoAportacionCrowd>().HasData(
            new MaestraEstadoAportacionCrowd { Id = 1, Codigo = "PENDIENTE", Nombre = "Pendiente", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoAportacionCrowd { Id = 2, Codigo = "AUTORIZADA", Nombre = "Autorizada", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoAportacionCrowd { Id = 3, Codigo = "CAPTURADA", Nombre = "Capturada", Orden = 3, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoAportacionCrowd { Id = 4, Codigo = "CANCELADA", Nombre = "Cancelada", Orden = 4, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoAportacionCrowd { Id = 5, Codigo = "REEMBOLSADA", Nombre = "Reembolsada", Orden = 5, EsActivo = true, FechaCreacion = seedDate }
        );

        // MaestraEstadoPayoutCrowd
        modelBuilder.Entity<MaestraEstadoPayoutCrowd>().HasData(
            new MaestraEstadoPayoutCrowd { Id = 1, Codigo = "PENDIENTE", Nombre = "Pendiente", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoPayoutCrowd { Id = 2, Codigo = "EN_PROCESO", Nombre = "En Proceso", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoPayoutCrowd { Id = 3, Codigo = "COMPLETADO", Nombre = "Completado", Orden = 3, EsActivo = true, FechaCreacion = seedDate },
            new MaestraEstadoPayoutCrowd { Id = 4, Codigo = "FALLIDO", Nombre = "Fallido", Orden = 4, EsActivo = true, FechaCreacion = seedDate }
        );
    }
}
