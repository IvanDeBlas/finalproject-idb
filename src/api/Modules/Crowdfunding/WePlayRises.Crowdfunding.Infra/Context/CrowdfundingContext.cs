using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Infra.Context;

public class CrowdfundingContext : CoreDbContext
{
    public CrowdfundingContext(DbContextOptions<CrowdfundingContext> options) : base(options)
    {
    }

    #region DbSets

    public DbSet<CampaniaCrowdfunding> Campanias => Set<CampaniaCrowdfunding>();
    public DbSet<CampaniaCrowdfundingReward> Rewards => Set<CampaniaCrowdfundingReward>();
    public DbSet<PedidoCrowdfunding> Pedidos => Set<PedidoCrowdfunding>();
    public DbSet<PedidoCrowdfundingLinea> PedidoLineas => Set<PedidoCrowdfundingLinea>();
    public DbSet<AportacionCrowdfunding> Aportaciones => Set<AportacionCrowdfunding>();
    public DbSet<ArtistaPayoutCuenta> ArtistaPayoutCuentas => Set<ArtistaPayoutCuenta>();
    public DbSet<CampaniaCrowdfundingPayout> Payouts => Set<CampaniaCrowdfundingPayout>();
    public DbSet<CampaniaCrowdfundingUpdate> Updates => Set<CampaniaCrowdfundingUpdate>();
    public DbSet<CampaniaCrowdfundingComentario> Comentarios => Set<CampaniaCrowdfundingComentario>();
    public DbSet<CampaniaCrowdfundingStretchGoal> StretchGoals => Set<CampaniaCrowdfundingStretchGoal>();
    public DbSet<ArtistaMembershipPlan> MembershipPlans => Set<ArtistaMembershipPlan>();
    public DbSet<ArtistaMembershipSuscripcion> MembershipSuscripciones => Set<ArtistaMembershipSuscripcion>();
    public DbSet<ArtistaMembershipPago> MembershipPagos => Set<ArtistaMembershipPago>();

    #endregion

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure Strongly Typed ID converters
        configurationBuilder.Properties<CampaniaCrowdfundingId>()
            .HaveConversion<CampaniaCrowdfundingId.EfCoreConverter>();

        configurationBuilder.Properties<CampaniaCrowdfundingRewardId>()
            .HaveConversion<CampaniaCrowdfundingRewardId.EfCoreConverter>();

        configurationBuilder.Properties<PedidoCrowdfundingId>()
            .HaveConversion<PedidoCrowdfundingId.EfCoreConverter>();

        configurationBuilder.Properties<AportacionCrowdfundingId>()
            .HaveConversion<AportacionCrowdfundingId.EfCoreConverter>();

        // IDs from other modules (for FK references)
        configurationBuilder.Properties<ArtistaId>()
            .HaveConversion<ArtistaId.EfCoreConverter>();

        configurationBuilder.Properties<FanProfileId>()
            .HaveConversion<FanProfileId.EfCoreConverter>();

        configurationBuilder.Properties<ProyectoArtisticoId>()
            .HaveConversion<ProyectoArtisticoId.EfCoreConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CampaniaCrowdfunding
        modelBuilder.Entity<CampaniaCrowdfunding>(entity =>
        {
            entity.ToTable("CampaniaCrowdfunding");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.ProyectoArtisticoId).HasColumnName("ProyectoArtistico_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Subtitulo).HasMaxLength(300);
            entity.Property(e => e.DescripcionCorta).HasMaxLength(500);
            entity.Property(e => e.VideoPrincipalUrl).HasMaxLength(500);
            entity.Property(e => e.ImagenPrincipalUrl).HasMaxLength(500);
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.ImporteObjetivo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteMinimo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImportePledgedActual).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoFinanciacionId).HasColumnName("TipoFinanciacion_Id");
            entity.Property(e => e.EstadoCampaniaId).HasColumnName("EstadoCampania_Id");
            entity.Property(e => e.PorcentajeComisionPlataforma).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FechaInicio).HasPrecision(3);
            entity.Property(e => e.FechaFin).HasPrecision(3);
            entity.Property(e => e.FechaPublicacion).HasPrecision(3);
            entity.Property(e => e.FechaCierre).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.ArtistaId, e.EstadoCampaniaId })
                .HasDatabaseName("IX_CampaniaCrowdfunding_Artista_Estado");

            entity.HasMany(e => e.Rewards).WithOne(e => e.Campania)
                .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Pedidos).WithOne(e => e.Campania)
                .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Payouts).WithOne(e => e.Campania)
                .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Updates).WithOne(e => e.Campania)
                .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Comentarios).WithOne(e => e.Campania)
                .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.StretchGoals).WithOne(e => e.Campania)
                .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
        });

        // CampaniaCrowdfundingReward
        modelBuilder.Entity<CampaniaCrowdfundingReward>(entity =>
        {
            entity.ToTable("CampaniaCrowdfunding_Reward");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CampaniaId).HasColumnName("Campania_Id");
            entity.Property(e => e.TipoRewardId).HasColumnName("TipoReward_Id");
            entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ImporteMinimo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.TiempoEntregaEstimado).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.CampaniaId, e.Orden })
                .HasDatabaseName("IX_CampaniaCrowdfunding_Reward_Campania_Orden");

            entity.HasMany(e => e.Lineas).WithOne(e => e.Reward)
                .HasForeignKey(e => e.RewardId).OnDelete(DeleteBehavior.Restrict);
        });

        // PedidoCrowdfunding
        modelBuilder.Entity<PedidoCrowdfunding>(entity =>
        {
            entity.ToTable("PedidoCrowdfunding");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CampaniaId).HasColumnName("Campania_Id");
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.FanProfileId).HasColumnName("FanProfile_Id");
            entity.Property(e => e.EstadoPedidoId).HasColumnName("EstadoPedido_Id");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.ImporteSubtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImportePropina).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteEnvio).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteImpuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ComentarioBacker).HasMaxLength(500);
            entity.Property(e => e.DireccionEnvioId).HasColumnName("DireccionEnvio_Id");
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.CampaniaId, e.EstadoPedidoId })
                .HasDatabaseName("IX_PedidoCrowdfunding_Campania_Estado");

            entity.HasMany(e => e.Lineas).WithOne(e => e.PedidoCrowdfunding)
                .HasForeignKey(e => e.PedidoCrowdfundingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Aportaciones).WithOne(e => e.PedidoCrowdfunding)
                .HasForeignKey(e => e.PedidoCrowdfundingId).OnDelete(DeleteBehavior.Cascade);
        });

        // PedidoCrowdfundingLinea
        modelBuilder.Entity<PedidoCrowdfundingLinea>(entity =>
        {
            entity.ToTable("PedidoCrowdfunding_Linea");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PedidoCrowdfundingId).HasColumnName("PedidoCrowdfunding_Id");
            entity.Property(e => e.RewardId).HasColumnName("Reward_Id");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteLinea).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaCreacion).HasPrecision(3);

            entity.HasIndex(e => e.PedidoCrowdfundingId)
                .HasDatabaseName("IX_PedidoCrowdfunding_Linea_Pedido");
        });

        // AportacionCrowdfunding
        modelBuilder.Entity<AportacionCrowdfunding>(entity =>
        {
            entity.ToTable("AportacionCrowdfunding");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PedidoCrowdfundingId).HasColumnName("PedidoCrowdfunding_Id");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.MetodoPagoId).HasColumnName("MetodoPago_Id");
            entity.Property(e => e.EstadoAportacionId).HasColumnName("EstadoAportacion_Id");
            entity.Property(e => e.ImporteTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteImpuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteComisionPlataforma).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteComisionPasarela).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteNetoArtista).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CodigoOperacionPasarela).HasMaxLength(100);
            entity.Property(e => e.CodigoOperacionProveedor).HasMaxLength(100);
            entity.Property(e => e.FechaAutorizacion).HasPrecision(3);
            entity.Property(e => e.FechaCaptura).HasPrecision(3);
            entity.Property(e => e.FechaCancelacion).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.PedidoCrowdfundingId, e.EstadoAportacionId })
                .HasDatabaseName("IX_AportacionCrowdfunding_Pedido_Estado");
        });

        // ArtistaPayoutCuenta
        modelBuilder.Entity<ArtistaPayoutCuenta>(entity =>
        {
            entity.ToTable("Artista_PayoutCuenta");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.NombreCuenta).HasMaxLength(200).IsRequired();
            entity.Property(e => e.TipoCuenta).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProveedorPayout).HasMaxLength(50);
            entity.Property(e => e.IdentificadorExterno).HasMaxLength(200);
            entity.Property(e => e.NombreTitular).HasMaxLength(200);
            entity.Property(e => e.Iban).HasColumnName("IBAN").HasMaxLength(34);
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaBaja).HasPrecision(3);

            entity.HasIndex(e => new { e.ArtistaId, e.EsPorDefecto })
                .HasDatabaseName("IX_Artista_PayoutCuenta_Artista_Id")
                .IsDescending(false, true);

            entity.HasMany(e => e.Payouts).WithOne(e => e.ArtistaPayoutCuenta)
                .HasForeignKey(e => e.ArtistaPayoutCuentaId).OnDelete(DeleteBehavior.Restrict);
        });

        // CampaniaCrowdfundingPayout
        modelBuilder.Entity<CampaniaCrowdfundingPayout>(entity =>
        {
            entity.ToTable("CampaniaCrowdfunding_Payout");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CampaniaId).HasColumnName("Campania_Id");
            entity.Property(e => e.ArtistaPayoutCuentaId).HasColumnName("ArtistaPayoutCuenta_Id");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.ImporteBruto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteComisionPlataforma).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteComisionPasarela).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteImpuestosRetenidos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteNetoArtista).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstadoPayoutId).HasColumnName("EstadoPayout_Id");
            entity.Property(e => e.FechaProgramada).HasPrecision(3);
            entity.Property(e => e.FechaEjecucion).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);
            entity.Property(e => e.NotasInternas).HasMaxLength(500);

            entity.HasIndex(e => new { e.CampaniaId, e.EstadoPayoutId })
                .HasDatabaseName("IX_CampaniaCrowdfunding_Payout_Campania_Estado");
        });

        // CampaniaCrowdfundingUpdate
        modelBuilder.Entity<CampaniaCrowdfundingUpdate>(entity =>
        {
            entity.ToTable("CampaniaCrowdfunding_Update");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CampaniaId).HasColumnName("Campania_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Contenido).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);
        });

        // CampaniaCrowdfundingComentario
        modelBuilder.Entity<CampaniaCrowdfundingComentario>(entity =>
        {
            entity.ToTable("CampaniaCrowdfunding_Comentario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CampaniaId).HasColumnName("Campania_Id");
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.ComentarioPadreId).HasColumnName("ComentarioPadre_Id");
            entity.Property(e => e.Contenido).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasOne(e => e.ComentarioPadre)
                .WithMany(e => e.Respuestas)
                .HasForeignKey(e => e.ComentarioPadreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CampaniaCrowdfundingStretchGoal
        modelBuilder.Entity<CampaniaCrowdfundingStretchGoal>(entity =>
        {
            entity.ToTable("CampaniaCrowdfunding_StretchGoal");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CampaniaId).HasColumnName("Campania_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ImporteObjetivo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.FechaAlcanzado).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // ArtistaMembershipPlan
        modelBuilder.Entity<ArtistaMembershipPlan>(entity =>
        {
            entity.ToTable("Artista_MembershipPlan");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.NombrePlan).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ImporteMensual).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.ArtistaId, e.NombrePlan })
                .HasDatabaseName("IX_Artista_MembershipPlan_Artista_NombrePlan")
                .IsUnique();

            entity.HasMany(e => e.Suscripciones).WithOne(e => e.MembershipPlan)
                .HasForeignKey(e => e.MembershipPlanId).OnDelete(DeleteBehavior.Restrict);
        });

        // ArtistaMembershipSuscripcion
        modelBuilder.Entity<ArtistaMembershipSuscripcion>(entity =>
        {
            entity.ToTable("Artista_MembershipSuscripcion");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.MembershipPlanId).HasColumnName("MembershipPlan_Id");
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.FechaInicio).HasPrecision(3);
            entity.Property(e => e.FechaFin).HasPrecision(3);
            entity.Property(e => e.ProximoCargo).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.ArtistaId, e.UserId, e.EsActiva })
                .HasDatabaseName("IX_Artista_MembershipSuscripcion_Artista_User")
                .IsDescending(false, false, true);

            entity.HasMany(e => e.Pagos).WithOne(e => e.MembershipSuscripcion)
                .HasForeignKey(e => e.MembershipSuscripcionId).OnDelete(DeleteBehavior.Cascade);
        });

        // ArtistaMembershipPago
        modelBuilder.Entity<ArtistaMembershipPago>(entity =>
        {
            entity.ToTable("Artista_MembershipPago");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MembershipSuscripcionId).HasColumnName("MembershipSuscripcion_Id");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.MetodoPagoId).HasColumnName("MetodoPago_Id");
            entity.Property(e => e.EstadoAportacionId).HasColumnName("EstadoAportacion_Id");
            entity.Property(e => e.ImporteTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteImpuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteComisionPlataforma).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteComisionPasarela).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteNetoArtista).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CodigoOperacionPasarela).HasMaxLength(100);
            entity.Property(e => e.CodigoOperacionProveedor).HasMaxLength(100);
            entity.Property(e => e.FechaAutorizacion).HasPrecision(3);
            entity.Property(e => e.FechaCaptura).HasPrecision(3);
            entity.Property(e => e.FechaCancelacion).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.MembershipSuscripcionId, e.EstadoAportacionId })
                .HasDatabaseName("IX_Artista_MembershipPago_Suscripcion");
        });
    }
}
