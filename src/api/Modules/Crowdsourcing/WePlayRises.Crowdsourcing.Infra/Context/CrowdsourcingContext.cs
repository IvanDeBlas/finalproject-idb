using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.Context;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Data.Configurations;
using WePlayRises.Crowdsourcing.Infra.Data.Seeds;

namespace WePlayRises.Crowdsourcing.Infra.Context;

public class CrowdsourcingContext : CoreDbContext
{
    public CrowdsourcingContext(DbContextOptions<CrowdsourcingContext> options) : base(options)
    {
    }

    #region DbSets

    public DbSet<NecesidadCrowdsourcing> Necesidades => Set<NecesidadCrowdsourcing>();
    public DbSet<PropuestaCrowdsourcing> Propuestas => Set<PropuestaCrowdsourcing>();
    public DbSet<AcuerdoCrowdsourcing> Acuerdos => Set<AcuerdoCrowdsourcing>();
    public DbSet<AcuerdoCrowdsourcingMilestone> Milestones => Set<AcuerdoCrowdsourcingMilestone>();
    public DbSet<AcuerdoCrowdsourcingEntregable> Entregables => Set<AcuerdoCrowdsourcingEntregable>();
    public DbSet<ConversacionCrowdsourcing> Conversaciones => Set<ConversacionCrowdsourcing>();
    public DbSet<MensajeCrowdsourcing> Mensajes => Set<MensajeCrowdsourcing>();
    public DbSet<ValoracionCrowdsourcing> Valoraciones => Set<ValoracionCrowdsourcing>();

    // Templates & Maestras
    public DbSet<PlantillaProyecto> PlantillasProyecto => Set<PlantillaProyecto>();
    public DbSet<PlantillaProyectoNecesidad> PlantillasProyectoNecesidades => Set<PlantillaProyectoNecesidad>();
    public DbSet<MaestraRolProfesional> MaestrasRolProfesional => Set<MaestraRolProfesional>();
    public DbSet<MaestraCategoriaRol> MaestrasCategoriaRol => Set<MaestraCategoriaRol>();
    public DbSet<MaestraEstadoAcuerdo> MaestrasEstadoAcuerdo => Set<MaestraEstadoAcuerdo>();
    public DbSet<MaestraEstadoEntregable> MaestrasEstadoEntregable => Set<MaestraEstadoEntregable>();

    #endregion

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure Strongly Typed ID converters
        configurationBuilder.Properties<NecesidadCrowdsourcingId>()
            .HaveConversion<NecesidadCrowdsourcingId.EfCoreConverter>();

        configurationBuilder.Properties<PropuestaCrowdsourcingId>()
            .HaveConversion<PropuestaCrowdsourcingId.EfCoreConverter>();

        configurationBuilder.Properties<AcuerdoCrowdsourcingId>()
            .HaveConversion<AcuerdoCrowdsourcingId.EfCoreConverter>();

        configurationBuilder.Properties<PlantillaProyectoId>()
            .HaveConversion<PlantillaProyectoId.EfCoreConverter>();

        configurationBuilder.Properties<PlantillaProyectoNecesidadId>()
            .HaveConversion<PlantillaProyectoNecesidadId.EfCoreConverter>();

        // IDs from other modules (for FK references)
        configurationBuilder.Properties<ArtistaId>()
            .HaveConversion<ArtistaId.EfCoreConverter>();

        configurationBuilder.Properties<ProyectoArtisticoId>()
            .HaveConversion<ProyectoArtisticoId.EfCoreConverter>();

        configurationBuilder.Properties<PerfilProfesionalId>()
            .HaveConversion<PerfilProfesionalId.EfCoreConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NecesidadCrowdsourcing
        modelBuilder.Entity<NecesidadCrowdsourcing>(entity =>
        {
            entity.ToTable("NecesidadCrowdsourcing");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProyectoArtisticoId).HasColumnName("ProyectoArtistico_Id");
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.TipoNecesidadId).HasColumnName("TipoNecesidad_Id");
            entity.Property(e => e.EstadoNecesidadId).HasColumnName("EstadoNecesidad_Id");
            entity.Property(e => e.ModalidadTrabajoId).HasColumnName("ModalidadTrabajo_Id");
            entity.Property(e => e.PresupuestoMin).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PresupuestoMax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.UbicacionCiudad).HasMaxLength(100);
            entity.Property(e => e.UbicacionPais).HasMaxLength(100);
            entity.Property(e => e.MotivoCierre).HasMaxLength(500);
            entity.Property(e => e.FechaLimitePropuestas).HasPrecision(3);
            entity.Property(e => e.FechaInicioPrevista).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.ArtistaId, e.EstadoNecesidadId })
                .HasDatabaseName("IX_NecesidadCrowdsourcing_Artista_Estado");

            entity.HasMany(e => e.Propuestas).WithOne(e => e.Necesidad)
                .HasForeignKey(e => e.NecesidadId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Acuerdos).WithOne(e => e.Necesidad)
                .HasForeignKey(e => e.NecesidadId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Conversaciones).WithOne(e => e.Necesidad)
                .HasForeignKey(e => e.NecesidadId).OnDelete(DeleteBehavior.SetNull);
        });

        // PropuestaCrowdsourcing
        modelBuilder.Entity<PropuestaCrowdsourcing>(entity =>
        {
            entity.ToTable("PropuestaCrowdsourcing");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NecesidadId).HasColumnName("Necesidad_Id");
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.PerfilProfesionalId).HasColumnName("PerfilProfesional_Id");
            entity.Property(e => e.MensajePropuesta).HasColumnType("nvarchar(max)");
            entity.Property(e => e.PrecioPropuesto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.EstadoPropuestaId).HasColumnName("EstadoPropuesta_Id");
            entity.Property(e => e.MotivoRechazo).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.Property(e => e.AcuerdoId).HasColumnName("Acuerdo_Id_Ref");

            entity.HasIndex(e => new { e.NecesidadId, e.UserId })
                .HasDatabaseName("IX_PropuestaCrowdsourcing_Necesidad_User");

            entity.HasMany(e => e.Acuerdos).WithOne(e => e.Propuesta)
                .HasForeignKey(e => e.PropuestaId).OnDelete(DeleteBehavior.SetNull);
        });

        // AcuerdoCrowdsourcing
        modelBuilder.Entity<AcuerdoCrowdsourcing>(entity =>
        {
            entity.ToTable("AcuerdoCrowdsourcing");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NecesidadId).HasColumnName("Necesidad_Id");
            entity.Property(e => e.PropuestaId).HasColumnName("Propuesta_Id");
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.UserIdProveedor).HasMaxLength(450).IsRequired();
            entity.Property(e => e.PerfilProfesionalId).HasColumnName("PerfilProfesional_Id");
            entity.Property(e => e.TituloInterno).HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.EstadoAcuerdoId).HasColumnName("EstadoAcuerdo_Id");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.ImporteTotalPactado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteAnticipo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PorcentajeAnticipo).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FechaInicio).HasPrecision(3);
            entity.Property(e => e.FechaFinPrevista).HasPrecision(3);
            entity.Property(e => e.FechaFinReal).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasIndex(e => new { e.ArtistaId, e.EstadoAcuerdoId })
                .HasDatabaseName("IX_AcuerdoCrowdsourcing_Artista_Estado");
            entity.HasIndex(e => e.NecesidadId)
                .HasDatabaseName("IX_AcuerdoCrowdsourcing_Necesidad");

            entity.Property(e => e.MotivoCancelacion).HasMaxLength(1000);
            entity.Property(e => e.CanceladoPor).HasMaxLength(450);

            entity.HasMany(e => e.Milestones).WithOne(e => e.Acuerdo)
                .HasForeignKey(e => e.AcuerdoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Entregables).WithOne(e => e.Acuerdo)
                .HasForeignKey(e => e.AcuerdoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Conversaciones).WithOne(e => e.Acuerdo)
                .HasForeignKey(e => e.AcuerdoId).OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Valoraciones).WithOne(e => e.Acuerdo)
                .HasForeignKey(e => e.AcuerdoId).OnDelete(DeleteBehavior.Cascade);
        });

        // AcuerdoCrowdsourcingMilestone
        modelBuilder.Entity<AcuerdoCrowdsourcingMilestone>(entity =>
        {
            entity.ToTable("AcuerdoCrowdsourcing_Milestone");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AcuerdoId).HasColumnName("Acuerdo_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ImporteParcial).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PorcentajeParcial).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FechaLimite).HasPrecision(3);
            entity.Property(e => e.FechaCompletado).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);

            entity.HasMany(e => e.Entregables)
                .WithOne(e => e.Milestone)
                .HasForeignKey(e => e.MilestoneId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // AcuerdoCrowdsourcingEntregable
        modelBuilder.Entity<AcuerdoCrowdsourcingEntregable>(entity =>
        {
            entity.ToTable("AcuerdoCrowdsourcing_Entregable");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AcuerdoId).HasColumnName("Acuerdo_Id");
            entity.Property(e => e.MilestoneId).HasColumnName("Milestone_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.UrlRecurso).HasMaxLength(500);
            entity.Property(e => e.EstadoEntregableId).HasColumnName("EstadoEntregable_Id");
            entity.Property(e => e.FechaEntrega).HasPrecision(3);
            entity.Property(e => e.FechaAprobacion).HasPrecision(3);
            entity.Property(e => e.ComentarioAprobacion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ComentarioRechazo).HasMaxLength(500);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // ConversacionCrowdsourcing
        modelBuilder.Entity<ConversacionCrowdsourcing>(entity =>
        {
            entity.ToTable("ConversacionCrowdsourcing");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.NecesidadId).HasColumnName("Necesidad_Id");
            entity.Property(e => e.AcuerdoId).HasColumnName("Acuerdo_Id");
            entity.Property(e => e.UserIdCreador).HasColumnName("UserIdCreador").HasMaxLength(450).IsRequired();
            entity.Property(e => e.UserIdDestinatario).HasColumnName("UserIdDestinatario").HasMaxLength(450).IsRequired();
            entity.Property(e => e.Asunto).HasMaxLength(200).IsRequired();
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaUltimoMensaje).HasPrecision(3);

            entity.HasOne(e => e.Necesidad).WithMany(e => e.Conversaciones)
                .HasForeignKey(e => e.NecesidadId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Acuerdo).WithMany(e => e.Conversaciones)
                .HasForeignKey(e => e.AcuerdoId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Mensajes).WithOne(e => e.Conversacion)
                .HasForeignKey(e => e.ConversacionId).OnDelete(DeleteBehavior.Cascade);

            // Unique filtered indexes to prevent duplicate conversations
            entity.HasIndex(e => new { e.UserIdCreador, e.UserIdDestinatario, e.NecesidadId })
                .HasFilter("[Necesidad_Id] IS NOT NULL").IsUnique()
                .HasDatabaseName("UX_ConversacionCrowdsourcing_Usuarios_Necesidad");
            entity.HasIndex(e => new { e.UserIdCreador, e.UserIdDestinatario, e.AcuerdoId })
                .HasFilter("[Acuerdo_Id] IS NOT NULL").IsUnique()
                .HasDatabaseName("UX_ConversacionCrowdsourcing_Usuarios_Acuerdo");

            // Performance indexes
            entity.HasIndex(e => e.FechaUltimoMensaje)
                .HasDatabaseName("IX_ConversacionCrowdsourcing_FechaUltimoMensaje");
            entity.HasIndex(e => new { e.UserIdCreador, e.FechaUltimoMensaje })
                .HasDatabaseName("IX_ConversacionCrowdsourcing_Creador_Fecha");
            entity.HasIndex(e => new { e.UserIdDestinatario, e.FechaUltimoMensaje })
                .HasDatabaseName("IX_ConversacionCrowdsourcing_Destinatario_Fecha");
        });

        // MensajeCrowdsourcing
        modelBuilder.Entity<MensajeCrowdsourcing>(entity =>
        {
            entity.ToTable("MensajeCrowdsourcing");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ConversacionId).HasColumnName("Conversacion_Id");
            entity.Property(e => e.UserIdRemitente).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Contenido).HasMaxLength(5000).IsRequired();
            entity.Property(e => e.UrlAdjunto).HasMaxLength(2048);
            entity.Property(e => e.Leido).HasDefaultValue(false);
            entity.Property(e => e.FechaLeido).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);

            // Performance indexes
            entity.HasIndex(e => new { e.ConversacionId, e.Leido, e.UserIdRemitente })
                .HasDatabaseName("IX_MensajeCrowdsourcing_Conversacion_Leido_Remitente");
            entity.HasIndex(e => new { e.ConversacionId, e.FechaCreacion })
                .HasDatabaseName("IX_MensajeCrowdsourcing_Conversacion_Fecha");
        });

        // MaestraEstadoAcuerdo
        modelBuilder.Entity<MaestraEstadoAcuerdo>(entity =>
        {
            entity.ToTable("MaestraEstadoAcuerdo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200);

            entity.HasData(
                new MaestraEstadoAcuerdo { Id = 1, Nombre = "Activo", Descripcion = "Acuerdo en curso" },
                new MaestraEstadoAcuerdo { Id = 2, Nombre = "Completado", Descripcion = "Acuerdo finalizado exitosamente" },
                new MaestraEstadoAcuerdo { Id = 3, Nombre = "Cancelado", Descripcion = "Acuerdo cancelado" }
            );
        });

        // MaestraEstadoEntregable
        modelBuilder.Entity<MaestraEstadoEntregable>(entity =>
        {
            entity.ToTable("MaestraEstadoEntregable");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200);

            entity.HasData(
                new MaestraEstadoEntregable { Id = 1, Nombre = "Entregado", Descripcion = "Subido por el profesional, pendiente revision" },
                new MaestraEstadoEntregable { Id = 2, Nombre = "Aprobado", Descripcion = "Aprobado por el artista" },
                new MaestraEstadoEntregable { Id = 3, Nombre = "Rechazado", Descripcion = "Rechazado, requiere nueva version" }
            );
        });

        // Templates & Maestras (external configurations)
        modelBuilder.ApplyConfiguration(new PlantillaProyectoConfiguration());
        modelBuilder.ApplyConfiguration(new PlantillaProyectoNecesidadConfiguration());
        modelBuilder.ApplyConfiguration(new MaestraRolProfesionalConfiguration());
        modelBuilder.ApplyConfiguration(new MaestraCategoriaRolConfiguration());
        modelBuilder.ApplyConfiguration(new ValoracionCrowdsourcingConfiguration());

        // Seed data
        CrowdsourcingTemplatesSeedData.Seed(modelBuilder);
    }
}
