using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class MaestraRolProfesionalConfiguration : IEntityTypeConfiguration<MaestraRolProfesional>
{
    public void Configure(EntityTypeBuilder<MaestraRolProfesional> builder)
    {
        builder.ToTable("MaestrasRolProfesional");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(500);

        builder.Property(x => x.CategoriaRolId)
            .IsRequired();

        builder.Property(x => x.ModalidadCobro)
            .HasMaxLength(100);

        builder.Property(x => x.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.CategoriaRol)
            .WithMany(c => c.Roles)
            .HasForeignKey(x => x.CategoriaRolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CategoriaRolId);
        builder.HasIndex(x => x.Activo);
    }
}
