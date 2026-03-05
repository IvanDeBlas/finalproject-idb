using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class ValoracionCrowdsourcingConfiguration : IEntityTypeConfiguration<ValoracionCrowdsourcing>
{
    public void Configure(EntityTypeBuilder<ValoracionCrowdsourcing> builder)
    {
        builder.ToTable("ValoracionCrowdsourcing", t =>
        {
            t.HasCheckConstraint(
                "CK_ValoracionCrowdsourcing_Puntuacion_Rango",
                "Puntuacion >= 1 AND Puntuacion <= 5");
        });
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.AcuerdoId).HasColumnName("Acuerdo_Id");
        builder.Property(e => e.UserIdAutor).HasMaxLength(450).IsRequired();
        builder.Property(e => e.UserIdValorado).HasMaxLength(450).IsRequired();
        builder.Property(e => e.Puntuacion).IsRequired().HasColumnType("tinyint");
        builder.Property(e => e.TipoValoracionId).HasColumnName("TipoValoracion_Id").IsRequired(false);
        builder.Property(e => e.Comentario).HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.FechaCreacion).IsRequired().HasPrecision(3);

        // Unique constraint: one valoracion per user per acuerdo
        builder.HasIndex(e => new { e.AcuerdoId, e.UserIdAutor })
            .IsUnique()
            .HasDatabaseName("UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor");

        // Performance indexes
        builder.HasIndex(e => e.UserIdValorado)
            .HasDatabaseName("IX_ValoracionCrowdsourcing_UserIdValorado");

        builder.HasIndex(e => new { e.UserIdValorado, e.FechaCreacion })
            .HasDatabaseName("IX_ValoracionCrowdsourcing_UserIdValorado_FechaCreacion");
    }
}
