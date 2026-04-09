using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class MaestraCategoriaRolConfiguration : IEntityTypeConfiguration<MaestraCategoriaRol>
{
    public void Configure(EntityTypeBuilder<MaestraCategoriaRol> builder)
    {
        builder.ToTable("MaestrasCategoriaRol");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Icono)
            .HasMaxLength(50);

        builder.Property(x => x.Orden)
            .IsRequired();

        builder.HasIndex(x => x.Orden);
    }
}
