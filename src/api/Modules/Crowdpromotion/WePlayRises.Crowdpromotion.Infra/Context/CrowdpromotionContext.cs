using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Infra.Context;

public class CrowdpromotionContext : CoreDbContext
{
    public CrowdpromotionContext(DbContextOptions<CrowdpromotionContext> options) : base(options)
    {
    }

    #region DbSets

    public DbSet<PromoPrograma> Programas => Set<PromoPrograma>();
    public DbSet<Promotor> Promotores => Set<Promotor>();
    public DbSet<PromoProgramaPromotor> ProgramaPromotores => Set<PromoProgramaPromotor>();
    public DbSet<PromoTarea> Tareas => Set<PromoTarea>();
    public DbSet<PromoTareaPromotor> TareaPromotores => Set<PromoTareaPromotor>();
    public DbSet<PromoEvento> Eventos => Set<PromoEvento>();
    public DbSet<PromotorWallet> Wallets => Set<PromotorWallet>();
    public DbSet<PromotorWalletTransaccion> WalletTransacciones => Set<PromotorWalletTransaccion>();

    #endregion

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure Strongly Typed ID converters - Main entities
        configurationBuilder.Properties<PromoProgramaId>()
            .HaveConversion<PromoProgramaId.EfCoreConverter>();

        configurationBuilder.Properties<PromotorId>()
            .HaveConversion<PromotorId.EfCoreConverter>();

        // IDs from other modules (for FK references)
        configurationBuilder.Properties<ArtistaId>()
            .HaveConversion<ArtistaId.EfCoreConverter>();

        configurationBuilder.Properties<ProyectoArtisticoId>()
            .HaveConversion<ProyectoArtisticoId.EfCoreConverter>();

        configurationBuilder.Properties<CampaniaCrowdfundingId>()
            .HaveConversion<CampaniaCrowdfundingId.EfCoreConverter>();

        configurationBuilder.Properties<FanProfileId>()
            .HaveConversion<FanProfileId.EfCoreConverter>();

        configurationBuilder.Properties<PedidoCrowdfundingId>()
            .HaveConversion<PedidoCrowdfundingId.EfCoreConverter>();

        configurationBuilder.Properties<AportacionCrowdfundingId>()
            .HaveConversion<AportacionCrowdfundingId.EfCoreConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PromoPrograma
        modelBuilder.Entity<PromoPrograma>(entity =>
        {
            entity.ToTable("PromoPrograma");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.ProyectoArtisticoId).HasColumnName("ProyectoArtistico_Id");
            entity.Property(e => e.CampaniaCrowdfundingId).HasColumnName("CampaniaCrowdfunding_Id");
            entity.Property(e => e.TipoPromoId).HasColumnName("TipoPromo_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.PresupuestoTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ComisionPorConversion).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ComisionPorClick).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UrlLanding).HasMaxLength(500);
            entity.Property(e => e.CodigoTrackingBase).HasMaxLength(50);
            entity.Property(e => e.ImporteComisionPorcentaje).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ImporteComisionFija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaInicio).HasPrecision(3);
            entity.Property(e => e.FechaFin).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.ArtistaId, e.EsActivo })
                .HasDatabaseName("IX_PromoPrograma_Artista_Activo");

            entity.HasIndex(e => new { e.ArtistaId, e.CodigoTrackingBase })
                .IsUnique()
                .HasFilter("[CodigoTrackingBase] IS NOT NULL")
                .HasDatabaseName("IX_PromoPrograma_Artista_CodigoTracking");

            entity.HasMany(e => e.Promotores).WithOne(e => e.Programa)
                .HasForeignKey(e => e.ProgramaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Tareas).WithOne(e => e.Programa)
                .HasForeignKey(e => e.ProgramaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Eventos).WithOne(e => e.Programa)
                .HasForeignKey(e => e.ProgramaId).OnDelete(DeleteBehavior.SetNull);
        });

        // Promotor
        modelBuilder.Entity<Promotor>(entity =>
        {
            entity.ToTable("Promotor");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoPromotorId).HasColumnName("TipoPromotor_Id");
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.FanProfileId).HasColumnName("FanProfile_Id");
            entity.Property(e => e.NombrePublico).HasMaxLength(200).IsRequired();
            entity.Property(e => e.EmailContacto).HasMaxLength(200);
            entity.Property(e => e.UrlSitioWeb).HasMaxLength(300);
            entity.Property(e => e.UrlInstagram).HasMaxLength(300);
            entity.Property(e => e.UrlTikTok).HasMaxLength(300);
            entity.Property(e => e.UrlTwitter).HasMaxLength(300);
            entity.Property(e => e.UrlYouTube).HasMaxLength(300);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => e.UserId).IsUnique()
                .HasDatabaseName("IX_Promotor_UserId");

            entity.HasMany(e => e.Programas).WithOne(e => e.Promotor)
                .HasForeignKey(e => e.PromotorId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Wallets).WithOne(e => e.Promotor)
                .HasForeignKey(e => e.PromotorId).OnDelete(DeleteBehavior.Cascade);
        });

        // PromoProgramaPromotor (join table)
        modelBuilder.Entity<PromoProgramaPromotor>(entity =>
        {
            entity.ToTable("PromoProgramaPromotor");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProgramaId).HasColumnName("Programa_Id");
            entity.Property(e => e.PromotorId).HasColumnName("Promotor_Id");
            entity.Property(e => e.CodigoReferido).HasMaxLength(50);
            entity.Property(e => e.UrlReferido).HasMaxLength(500);
            entity.Property(e => e.TotalComisionesGeneradas).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EsAprobado).HasDefaultValue(false);
            entity.Property(e => e.EsBloqueado).HasDefaultValue(false);
            entity.Property(e => e.FechaInscripcion).HasPrecision(3);
            entity.Property(e => e.FechaBaja).HasPrecision(3);

            entity.HasIndex(e => e.CodigoReferido)
                .HasDatabaseName("IX_PromoProgramaPromotor_CodigoReferido");
            entity.HasIndex(e => new { e.ProgramaId, e.PromotorId })
                .IsUnique()
                .HasDatabaseName("IX_PromoProgramaPromotor_Programa_Promotor");

            entity.HasMany(e => e.TareasAsignadas).WithOne(e => e.ProgramaPromotor)
                .HasForeignKey(e => e.ProgramaPromotorId).OnDelete(DeleteBehavior.Cascade);
        });

        // PromoTarea
        modelBuilder.Entity<PromoTarea>(entity =>
        {
            entity.ToTable("PromoTarea");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProgramaId).HasColumnName("Programa_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.InstruccionesUrl).HasMaxLength(500);
            entity.Property(e => e.TipoRewardId).HasColumnName("TipoReward_Id");
            entity.Property(e => e.ImporteRecompensa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.TipoEventoPromoId).HasColumnName("TipoEventoPromo_Id").IsRequired();
            entity.Property(e => e.EsRepetible).HasDefaultValue(true);
            entity.Property(e => e.FechaInicio).HasPrecision(3);
            entity.Property(e => e.FechaFin).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);

            entity.HasMany(e => e.TareasPromotor).WithOne(e => e.Tarea)
                .HasForeignKey(e => e.TareaId).OnDelete(DeleteBehavior.Cascade);
        });

        // PromoTareaPromotor
        modelBuilder.Entity<PromoTareaPromotor>(entity =>
        {
            entity.ToTable("PromoTareaPromotor");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TareaId).HasColumnName("Tarea_Id");
            entity.Property(e => e.ProgramaPromotorId).HasColumnName("ProgramaPromotor_Id");
            entity.Property(e => e.EstadoTareaId).HasColumnName("EstadoTarea_Id");
            entity.Property(e => e.UrlPruebaCompletado).HasMaxLength(2048);
            entity.Property(e => e.ComentarioPromotor).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ComentarioValidacion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.FechaCompletado).HasPrecision(3);
            entity.Property(e => e.FechaValidado).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // PromoEvento
        modelBuilder.Entity<PromoEvento>(entity =>
        {
            entity.ToTable("PromoEvento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProgramaId).HasColumnName("Programa_Id");
            entity.Property(e => e.PromotorId).HasColumnName("Promotor_Id");
            entity.Property(e => e.CampaniaCrowdfundingId).HasColumnName("CampaniaCrowdfunding_Id");
            entity.Property(e => e.PedidoCrowdfundingId).HasColumnName("PedidoCrowdfunding_Id");
            entity.Property(e => e.AportacionCrowdfundingId).HasColumnName("AportacionCrowdfunding_Id");
            entity.Property(e => e.TipoEventoId).HasColumnName("TipoEvento_Id");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.ImporteAsociado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CodigoReferido).HasMaxLength(50);
            entity.Property(e => e.IpOrigen).HasMaxLength(45);
            entity.Property(e => e.UserAgentOrigen).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);

            // New fields for tracking - US-CP-05
            entity.Property(e => e.PromoProgramaPromotorId).HasColumnName("PromoPrograma_Promotor_Id");
            entity.Property(e => e.UserIdAfectado).HasMaxLength(450);
            entity.Property(e => e.UrlOrigen).HasMaxLength(2048);
            entity.Property(e => e.UrlReferer).HasMaxLength(2048);
            entity.Property(e => e.UtmSource).HasMaxLength(100);
            entity.Property(e => e.UtmMedium).HasMaxLength(100);
            entity.Property(e => e.UtmCampaign).HasMaxLength(100);

            entity.HasIndex(e => e.CodigoReferido)
                .HasDatabaseName("IX_PromoEvento_CodigoReferido");
            entity.HasIndex(e => e.FechaCreacion)
                .HasDatabaseName("IX_PromoEvento_FechaCreacion");

            // New indexes for dashboard queries - US-CP-05
            entity.HasIndex(e => new { e.ProgramaId, e.FechaCreacion })
                .HasDatabaseName("IX_PromoEvento_Programa_Fecha");
            entity.HasIndex(e => new { e.PromotorId, e.TipoEventoId, e.FechaCreacion })
                .HasDatabaseName("IX_PromoEvento_Promotor_Tipo_Fecha");
            entity.HasIndex(e => e.PromoProgramaPromotorId)
                .HasDatabaseName("IX_PromoEvento_ProgramaPromotor");

            entity.HasOne(e => e.Promotor).WithMany()
                .HasForeignKey(e => e.PromotorId).OnDelete(DeleteBehavior.SetNull);
        });

        // PromotorWallet
        modelBuilder.Entity<PromotorWallet>(entity =>
        {
            entity.ToTable("PromotorWallet");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PromotorId).HasColumnName("Promotor_Id");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.SaldoDisponible).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaldoPendiente).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalGanado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalRetirado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasIndex(e => new { e.PromotorId, e.MonedaId })
                .IsUnique()
                .HasDatabaseName("IX_PromotorWallet_Promotor_Moneda");

            entity.HasMany(e => e.Transacciones).WithOne(e => e.Wallet)
                .HasForeignKey(e => e.WalletId).OnDelete(DeleteBehavior.Cascade);
        });

        // PromotorWalletTransaccion
        modelBuilder.Entity<PromotorWalletTransaccion>(entity =>
        {
            entity.ToTable("PromotorWalletTransaccion");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WalletId).HasColumnName("Wallet_Id");
            entity.Property(e => e.TipoRewardId).HasColumnName("TipoReward_Id");
            entity.Property(e => e.EstadoTransaccionId).HasColumnName("EstadoTransaccion_Id");
            entity.Property(e => e.CampaniaPayoutId).HasColumnName("CampaniaPayout_Id");
            entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Concepto).HasMaxLength(200);
            entity.Property(e => e.ReferenciaExterna).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaProcesado).HasPrecision(3);

            // New fields - US-CP-06
            entity.Property(e => e.EsCredito).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.PromoEventoId).HasColumnName("PromoEvento_Id");
            entity.Property(e => e.Descripcion).HasMaxLength(500);

            // FK to PromoEvento
            entity.HasOne(e => e.PromoEvento)
                .WithMany()
                .HasForeignKey(e => e.PromoEventoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.FechaCreacion)
                .HasDatabaseName("IX_PromotorWalletTransaccion_FechaCreacion");

            // New composite indexes - US-CP-06
            entity.HasIndex(e => new { e.WalletId, e.EsCredito, e.EstadoTransaccionId })
                .HasDatabaseName("IX_PromotorWalletTransaccion_Wallet_EsCredito_Estado");
            entity.HasIndex(e => new { e.WalletId, e.FechaCreacion })
                .HasDatabaseName("IX_PromotorWalletTransaccion_Wallet_Fecha");
        });
    }
}
