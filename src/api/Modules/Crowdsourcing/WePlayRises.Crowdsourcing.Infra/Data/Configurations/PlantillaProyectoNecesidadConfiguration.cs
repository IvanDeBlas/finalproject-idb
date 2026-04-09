using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class PlantillaProyectoNecesidadConfiguration : IEntityTypeConfiguration<PlantillaProyectoNecesidad>
{
    public void Configure(EntityTypeBuilder<PlantillaProyectoNecesidad> builder)
    {
        builder.ToTable("PlantillasProyectoNecesidades");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Fase)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Titulo)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(1000);

        builder.Property(x => x.RolProfesionalId)
            .IsRequired();

        builder.Property(x => x.PrecioMinOrientativo)
            .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.PrecioMaxOrientativo)
            .HasColumnType("decimal(18, 2)");

        builder.Property(x => x.MonedaId)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.Prioridad)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Media");

        builder.Property(x => x.Orden)
            .IsRequired();

        builder.Property(x => x.FechaCreacion)
            .IsRequired()
            .HasPrecision(3);

        builder.HasOne(x => x.PlantillaProyecto)
            .WithMany(p => p.Necesidades)
            .HasForeignKey(x => x.PlantillaProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RolProfesional)
            .WithMany(r => r.PlantillasNecesidades)
            .HasForeignKey(x => x.RolProfesionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PlantillaProyectoId);
        builder.HasIndex(x => x.RolProfesionalId);
        builder.HasIndex(x => x.Prioridad);
    }
}
