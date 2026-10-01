using Instartups.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Infrastructure.Persistence.Configurations;

public class PerfilSeguidorConfig : BaseConf<PerfilSeguidorEntity>
{
    public override void Configure(EntityTypeBuilder<PerfilSeguidorEntity> builder)
    {
        base.Configure(builder);
        builder.ToTable("perfil_seguidores");

        builder.Property(ps => ps.SeguidorId)
            .HasColumnName("seguidor_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(ps => ps.SeguidoId)
            .HasColumnName("seguido_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(ps => ps.Seguidor)
            .WithMany()
            .HasForeignKey(ps => ps.SeguidorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.Seguido)
            .WithMany()
            .HasForeignKey(ps => ps.SeguidoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
