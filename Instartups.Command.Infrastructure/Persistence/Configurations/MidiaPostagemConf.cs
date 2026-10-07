using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Command.Infrastructure.Persistence.Configurations;

public class MidiaPostagemConf : BaseConf<MidiaPostagemEntity>
{
    public override void Configure(EntityTypeBuilder<MidiaPostagemEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("midias_postagens");

        builder.Property(mp => mp.UrlMidia)
            .HasColumnName("url_midia")
            .HasColumnType("varchar(400)")
            .IsRequired();

        builder.Property(mp => mp.TipoMidia)
            .HasConversion(
                tipo => (int)tipo,
                valor => (TiposMidiaEnum)valor
            )
            .HasColumnName("tipo_midia")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(mp => mp.PostagemId)
            .HasColumnName("postagem_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(mp => mp.Postagem)
            .WithMany()
            .HasForeignKey(mp => mp.PostagemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
