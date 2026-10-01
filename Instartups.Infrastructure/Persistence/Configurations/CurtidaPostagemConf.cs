using Instartups.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Infrastructure.Persistence.Configurations;

public class CurtidaPostagemConf : BaseConf<CurtidaPostagemEntity>
{
    public override void Configure(EntityTypeBuilder<CurtidaPostagemEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("curtidas_postagens");

        builder.Property(cp => cp.PostagemId)
            .HasColumnName("postagem_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(cp => cp.PerfilAutorCurtidaId)
            .HasColumnName("perfil_autor_curtida_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(cp => cp.Postagem)
            .WithMany()
            .HasForeignKey(cp => cp.PostagemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cp => cp.PerfilAutorCurtida)
            .WithMany()
            .HasForeignKey(cp => cp.PerfilAutorCurtidaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
