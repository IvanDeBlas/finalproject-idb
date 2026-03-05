using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class PlantillaProyectoConfiguration : IEntityTypeConfiguration<PlantillaProyecto>
{
    public void Configure(EntityTypeBuilder<PlantillaProyecto> builder)
    {
        builder.ToTable("PlantillasProyecto");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(1000);

        builder.Property(x => x.Icono)
            .HasMaxLength(50);

        builder.Property(x => x.Orden)
            .IsRequired();

        builder.Property(x => x.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.FechaCreacion)
            .IsRequired()
            .HasPrecision(3);

        builder.HasMany(x => x.Necesidades)
            .WithOne(n => n.PlantillaProyecto)
            .HasForeignKey(n => n.PlantillaProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Activo);
        builder.HasIndex(x => x.Orden);
    }
}
