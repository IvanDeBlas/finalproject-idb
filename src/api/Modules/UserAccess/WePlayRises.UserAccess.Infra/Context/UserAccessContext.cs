using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Infra.Context;

public class UserAccessContext : IdentityDbContext<IdentityUser>
{
    public UserAccessContext(DbContextOptions<UserAccessContext> options) : base(options)
    {
    }

    #region DbSets

    public DbSet<Artista> Artistas => Set<Artista>();
    public DbSet<FanProfile> FanProfiles => Set<FanProfile>();
    public DbSet<ArtistaMiembro> ArtistaMiembros => Set<ArtistaMiembro>();
    public DbSet<ArtistaFan> ArtistaFans => Set<ArtistaFan>();
    public DbSet<DireccionPostal> DireccionesPostales => Set<DireccionPostal>();
    public DbSet<ProyectoArtistico> ProyectosArtisticos => Set<ProyectoArtistico>();
    public DbSet<PerfilProfesional> PerfilesProfesionales => Set<PerfilProfesional>();
    public DbSet<PerfilProfesionalSkill> PerfilProfesionalSkills => Set<PerfilProfesionalSkill>();
    public DbSet<PerfilProfesionalPortfolioItem> PerfilProfesionalPortfolioItems => Set<PerfilProfesionalPortfolioItem>();

    #endregion

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure Strongly Typed ID converters
        configurationBuilder.Properties<ArtistaId>()
            .HaveConversion<ArtistaId.EfCoreConverter>();

        configurationBuilder.Properties<FanProfileId>()
            .HaveConversion<FanProfileId.EfCoreConverter>();

        configurationBuilder.Properties<ProyectoArtisticoId>()
            .HaveConversion<ProyectoArtisticoId.EfCoreConverter>();

        configurationBuilder.Properties<PerfilProfesionalId>()
            .HaveConversion<PerfilProfesionalId.EfCoreConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Artista
        modelBuilder.Entity<Artista>(entity =>
        {
            entity.ToTable("Artista");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.UserIdPropietario).HasMaxLength(450).IsRequired();
            entity.HasIndex(e => e.UserIdPropietario).IsUnique();
            entity.Property(e => e.NombreArtistico).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Pais).HasMaxLength(100);
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.Property(e => e.ImagenPerfilUrl).HasMaxLength(500);
            entity.Property(e => e.UrlSitioWeb).HasMaxLength(300);
            entity.Property(e => e.UrlInstagram).HasMaxLength(300);
            entity.Property(e => e.UrlYouTube).HasMaxLength(300);
            entity.Property(e => e.UrlSpotify).HasMaxLength(300);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasMany(e => e.ArtistaMiembros)
                .WithOne(e => e.Artista)
                .HasForeignKey(e => e.ArtistaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ArtistaFans)
                .WithOne(e => e.Artista)
                .HasForeignKey(e => e.ArtistaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ProyectosArtisticos)
                .WithOne(e => e.Artista)
                .HasForeignKey(e => e.ArtistaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FanProfile
        modelBuilder.Entity<FanProfile>(entity =>
        {
            entity.ToTable("FanProfile");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Apodo).HasMaxLength(100);
            entity.Property(e => e.Pais).HasMaxLength(100);
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasMany(e => e.ArtistaFans)
                .WithOne(e => e.FanProfile)
                .HasForeignKey(e => e.FanProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ArtistaMiembro
        modelBuilder.Entity<ArtistaMiembro>(entity =>
        {
            entity.ToTable("Artista_Miembro");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.RolMiembroId).HasColumnName("RolMiembro_Id");
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaBaja).HasPrecision(3);
        });

        // ArtistaFan
        modelBuilder.Entity<ArtistaFan>(entity =>
        {
            entity.ToTable("Artista_Fan");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.FanProfileId).HasColumnName("FanProfile_Id");
            entity.Property(e => e.FechaInicio).HasPrecision(3);
            entity.Property(e => e.FechaFin).HasPrecision(3);
        });

        // DireccionPostal
        modelBuilder.Entity<DireccionPostal>(entity =>
        {
            entity.ToTable("DireccionPostal");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.NombreDestinatario).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Linea1).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Linea2).HasMaxLength(200);
            entity.Property(e => e.Ciudad).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Provincia).HasMaxLength(100);
            entity.Property(e => e.CodigoPostal).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Pais).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(50);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // ProyectoArtistico
        modelBuilder.Entity<ProyectoArtistico>(entity =>
        {
            entity.ToTable("ProyectoArtistico");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
            entity.Property(e => e.TipoProyectoId).HasColumnName("TipoProyecto_Id");
            entity.Property(e => e.EstadoProyectoId).HasColumnName("EstadoProyecto_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.DescripcionCorta).HasMaxLength(500);
            entity.Property(e => e.DescripcionLarga).HasColumnType("nvarchar(max)");
            entity.Property(e => e.UrlPortada).HasMaxLength(300);
            entity.Property(e => e.FechaInicio).HasPrecision(3);
            entity.Property(e => e.FechaFinPrevista).HasPrecision(3);
            entity.Property(e => e.FechaFinReal).HasPrecision(3);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);
        });

        // PerfilProfesional
        modelBuilder.Entity<PerfilProfesional>(entity =>
        {
            entity.ToTable("PerfilProfesional");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Titulo).HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.TarifaHora).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TarifaProyectoMin).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
            entity.Property(e => e.UrlPortfolio).HasMaxLength(300);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
            entity.Property(e => e.FechaActualizacion).HasPrecision(3);

            entity.HasMany(e => e.Skills)
                .WithOne(e => e.PerfilProfesional)
                .HasForeignKey(e => e.PerfilProfesionalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.PortfolioItems)
                .WithOne(e => e.PerfilProfesional)
                .HasForeignKey(e => e.PerfilProfesionalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PerfilProfesionalSkill
        modelBuilder.Entity<PerfilProfesionalSkill>(entity =>
        {
            entity.ToTable("PerfilProfesional_Skill");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PerfilProfesionalId).HasColumnName("PerfilProfesional_Id");
            entity.Property(e => e.TipoSkillId).HasColumnName("TipoSkill_Id");
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });

        // PerfilProfesionalPortfolioItem
        modelBuilder.Entity<PerfilProfesionalPortfolioItem>(entity =>
        {
            entity.ToTable("PerfilProfesional_PortfolioItem");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PerfilProfesionalId).HasColumnName("PerfilProfesional_Id");
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.UrlRecurso).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasPrecision(3);
        });
    }
}
